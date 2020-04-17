using DasKeyboardController2.Integration;
using DK4Q.Integration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;

namespace DK4Q.InfoProviders
{
    class CpuInfoProvider
    {
        private readonly PerformanceCounter _cpuCounter;

        internal CpuInfoProvider()
        {
            _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        }

        internal void Render(RgbClient client)
        {
            var cpu = getCpuUtilization();

            var backgroundColor = Color.White;
            var barStart = 2;
            var barLength = 12;
            var activeSlots = cpu / 100.0 * barLength;

            for (var i = 0; i < barLength; ++i)
            {
                Color color;
                if (i < 4)
                {
                    color = Color.FromArgb(0, 255, 0);
                }
                else if (i < 8)
                {
                    color = Color.Yellow;
                }
                else
                {
                    color = Color.Red;
                }

                if (i > activeSlots)
                {
                    color = backgroundColor;
                }

                client.SetKey(i + barStart, 0, color);
            }
        }

        private int getCpuUtilization()
        {
            return (int)Math.Round(_cpuCounter.NextValue());
        }
    }
}
