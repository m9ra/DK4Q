using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DK4Q.Integration
{
    class QToolsDriver
    {
        public static readonly int ZONE_WIDTH = 23;

        public static readonly int ZONE_HEIGHT = 6;

        private readonly static int WORKER_COUNT = 10;

        private readonly string signalApiUrl;

        private readonly BlockingCollection<string> _updatedKeys = new BlockingCollection<string>();

        private readonly Dictionary<string, KeyState> _desiredKeyboard = new Dictionary<string, KeyState>();

        private readonly Dictionary<string, KeyState> _actualKeyboard = new Dictionary<string, KeyState>();

        private volatile bool _isStopped = false;

        internal QToolsDriver(int port)
        {
            signalApiUrl = $"http://127.0.0.1:{port}/api/1.0/signals";
            for (var x = 0; x < ZONE_WIDTH; ++x)
            {
                for (var y = 0; y < ZONE_HEIGHT; ++y)
                {
                    var key = getKey(x, y);
                    _desiredKeyboard[key] = new KeyState();
                    _actualKeyboard[key] = new KeyState();
                }
            }

            for (var i = 0; i < WORKER_COUNT; ++i)
            {
                var th = new Thread(_worker);
                th.IsBackground = true;
                th.Start();
            }
        }

        public static Color ColorFromHSV(double hue, double saturation, double value)
        {
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            value = value * 255;
            int v = Convert.ToInt32(value);
            int p = Convert.ToInt32(value * (1 - saturation));
            int q = Convert.ToInt32(value * (1 - f * saturation));
            int t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

            if (hi == 0)
                return Color.FromArgb(255, v, t, p);
            else if (hi == 1)
                return Color.FromArgb(255, q, v, p);
            else if (hi == 2)
                return Color.FromArgb(255, p, v, t);
            else if (hi == 3)
                return Color.FromArgb(255, p, q, v);
            else if (hi == 4)
                return Color.FromArgb(255, t, p, v);
            else
                return Color.FromArgb(255, v, p, q);
        }

        internal void ForgetKeys()
        {
            foreach(var state in _actualKeyboard.Values)
            {
                state.Color = Color.Black;
                state.Effect = null;
            }
        }

        public static Color Jet(double v)
        {
            if (v < 0 || v > 1)
                throw new ArgumentOutOfRangeException("value");

            var r = 1.0;
            var g = 1.0;
            var b = 1.0;

            double dv;

            var vmin = 0.0;
            var vmax = 1.0;
            dv = vmax - vmin;

            if (v < (vmin + 0.25 * dv))
            {
                r = 0;
                g = 4 * (v - vmin) / dv;
            }
            else if (v < (vmin + 0.5 * dv))
            {
                r = 0;
                b = 1 + 4 * (vmin + 0.25 * dv - v) / dv;
            }
            else if (v < (vmin + 0.75 * dv))
            {
                r = 4 * (v - vmin - 0.5 * dv) / dv;
                b = 0;
            }
            else
            {
                g = 1 + 4 * (vmin + 0.75 * dv - v) / dv;
                b = 0;
            }

            return Color.FromArgb((int)Math.Round(r * 255), (int)Math.Round(g * 255), (int)Math.Round(b * 255));
        }

        public void SetAllKeys(Color color, string effect = KeyState.SET_COLOR)
        {
            for (var x = 0; x < ZONE_WIDTH; ++x)
            {
                for (var y = 0; y < ZONE_HEIGHT; ++y)
                {
                    SetKey(x, y, color, effect);
                }
            }
        }

        public void SetKey(int x, int y, Color color, string effect = KeyState.SET_COLOR)
        {
            var key = getKey(x, y);
            var keyState = _desiredKeyboard[key];

            if (keyState.Color == color && keyState.Effect == effect)
                return; // no change needed

            keyState.Color = color;
            keyState.Effect = effect;
            _updatedKeys.Add(key);
        }

        private string getKey(int x, int y)
        {
            return $"{x},{y}";
        }

        private void _worker()
        {
            while (!_isStopped)
            {
                var key = _updatedKeys.Take();
                var desiredState = _desiredKeyboard[key].CreateSnapshot();

                if (!_actualKeyboard.TryGetValue(key, out var actualState))
                {
                    lock (_actualKeyboard)
                    {
                        if (!_actualKeyboard.TryGetValue(key, out actualState))
                            _actualKeyboard[key] = actualState = new KeyState();
                    }
                }

                lock (actualState)
                {
                    if (actualState.IsSameAs(desiredState))
                        //nothing to change here
                        continue;

                    if (set(key, desiredState))
                    {
                        // report state change
                        actualState.UpdateBy(desiredState);
                    }
                    else
                    {
                        // change was not successful - try repeat
                        _updatedKeys.Add(key);
                    }
                }
            }
        }

        private bool set(string key, KeyState state)
        {
            var color = state.Color;
            var hexColor = "#" + color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");

            try
            {
                var response = sendSignal(key, hexColor, state.Effect);

                response.Wait();
                var responseText = response.Result.Content.ReadAsStringAsync();
                responseText.Wait();
                var responseTextResult = responseText.Result;

                Console.WriteLine(responseTextResult);
                return responseTextResult.StartsWith("{") && responseTextResult.EndsWith("}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Thread.Sleep(1000);
                return false;
            }
        }

        private Task<HttpResponseMessage> sendSignal(string key, string color, string effect)
        {
            var content = new Dictionary<string, string>
            {
                {"pid", "DK4QPID"},
                {"zoneId", key },
                {"color", color },
                {"effect", effect},
                //{"name", "KeyboardController "+Thread.CurrentThread.ManagedThreadId},
                //{"name", "KeyboardController"},
            };

            var json = JsonConvert.SerializeObject(content);
            var stringContent = new StringContent(json);
            stringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.ExpectContinue = false;

            return client.PostAsync(signalApiUrl, stringContent);
        }
    }
}
