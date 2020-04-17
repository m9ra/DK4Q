using DasKeyboardController2.Integration;
using DK4Q.Integration;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DasKeyboardController2.InfoProviders
{
    class ExtendedPingInfoProvider
    {
        private double[] _currentPings = new double[9];

        private int _currentPingIndex = 0;

        private readonly string _host;

        internal ExtendedPingInfoProvider(string host)
        {
            _host = host;

            var th = new Thread(_worker);
            th.IsBackground = true;
            th.Start();
        }

        internal void Render(RgbClient client)
        {
            var p = _currentPings.OrderByDescending(n=>n).ToArray();

            drawPing(client, 17, 2, p[0]);
            drawPing(client, 18, 2, p[1]);
            drawPing(client, 19, 2, p[2]);

            drawPing(client, 17, 3, p[3]);
            drawPing(client, 18, 3, p[4]);
            drawPing(client, 19, 3, p[5]);

            drawPing(client, 17, 4, p[6]);
            drawPing(client, 18, 4, p[7]);
            drawPing(client, 19, 4, p[8]);
        }

        private void drawPing(RgbClient client, int x, int y, double value)
        {

            string effect = KeyState.SET_COLOR;
            Color c;
            if (value < 20)
            {
                c = Color.FromArgb(0, 255, 0);
            }
            else if (value < 50)
            {
                c = Color.Yellow;
            }
            else if (value < 100)
            {
                c = Color.Orange;
            }
            else
            {
                effect = KeyState.BREATHE;
                c = Color.Red;
            }

            client.SetKey(x, y, c, effect);
        }

        private void _worker()
        {
            while (true)
            {
                double currentPing;
                try
                {
                    var ping = new Ping();
                    var reply = ping.Send(_host, 500);
                    if (reply.Status != IPStatus.Success)
                    {
                        throw new InvalidOperationException("ping was not successful due to " + reply.Status);
                    }

                    currentPing = reply.RoundtripTime;
                }
                catch (Exception)
                {
                    currentPing = double.PositiveInfinity;
                }

                _currentPings[_currentPingIndex] = currentPing;
                ++_currentPingIndex;

                if (_currentPingIndex >= _currentPings.Length)
                {
                    _currentPingIndex = 0;
                }

                Thread.Sleep(250);
            }
        }
    }
}
