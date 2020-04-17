using DasKeyboardController2.Integration;
using DK4Q.Integration;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;

namespace DK4Q.InfoProviders
{
    class PingInfoProvider
    {
        private readonly string _host;

        private double _currentPing = 0;

        internal PingInfoProvider(string host)
        {
            _host = host;

            var th = new Thread(_worker);
            th.IsBackground = true;
            th.Start();
        }

        internal void Render(RgbClient client)
        {
            string effect = KeyState.SET_COLOR;
            Color c;
            if (_currentPing < 15)
            {
                c = Color.FromArgb(0, 255, 0);
            }
            else if (_currentPing < 50)
            {
                c = Color.Yellow;
            }
            else if (_currentPing < 100)
            {
                c = Color.Orange;
            }
            else
            {
                effect = KeyState.BREATHE;
                c = Color.Red;
            }

            client.SetKey(13, 1, c, effect);
        }

        private void _worker()
        {
            var pingRepeatCount = 5;
            while (true)
            {
                try
                {
                    var timeAccumulator = 0L;
                    for (var i = 0; i < pingRepeatCount; ++i)
                    {
                        var ping = new Ping();
                        var reply = ping.Send(_host);
                        if (reply.Status != IPStatus.Success)
                        {
                            throw new InvalidOperationException("ping was not successful due to " + reply.Status);
                        }
                        timeAccumulator += reply.RoundtripTime;
                    }
                    _currentPing = timeAccumulator / pingRepeatCount;
                }
                catch (Exception)
                {
                    _currentPing = double.PositiveInfinity;
                }

                Thread.Sleep(2000);
            }
        }
    }
}
