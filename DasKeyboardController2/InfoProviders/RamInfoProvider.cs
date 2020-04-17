using DasKeyboardController2.Integration;
using DK4Q.Integration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace DK4Q.InfoProviders
{
    class RamInfoProvider
    {
        internal void Render(RgbClient client)
        {
            var availableMemory = PerformanceInfo.GetPhysicalAvailableMemoryInMiB();
            var totalMemory = PerformanceInfo.GetTotalMemoryInMiB();

            var ramUsage = 100 - 100.0 * availableMemory / totalMemory;
            Color ramColor;
            if (ramUsage < 50)
            {
                ramColor = Color.FromArgb(0, 255, 0);
            }
            else if (ramUsage < 75)
            {
                ramColor = Color.Yellow;
            }
            else if (ramUsage < 85)
            {
                ramColor = Color.Orange;
            }
            else
            {
                ramColor = Color.Red;
            }

            client.SetKey(0, 2, ramColor);
        }
    }

    public static class PerformanceInfo
    {
        [DllImport("psapi.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetPerformanceInfo([Out] out PerformanceInformation PerformanceInformation, [In] int Size);

        [StructLayout(LayoutKind.Sequential)]
        public struct PerformanceInformation
        {
            public int Size;
            public IntPtr CommitTotal;
            public IntPtr CommitLimit;
            public IntPtr CommitPeak;
            public IntPtr PhysicalTotal;
            public IntPtr PhysicalAvailable;
            public IntPtr SystemCache;
            public IntPtr KernelTotal;
            public IntPtr KernelPaged;
            public IntPtr KernelNonPaged;
            public IntPtr PageSize;
            public int HandlesCount;
            public int ProcessCount;
            public int ThreadCount;
        }

        public static Int64 GetPhysicalAvailableMemoryInMiB()
        {
            var pi = new PerformanceInformation();
            if (GetPerformanceInfo(out pi, Marshal.SizeOf(pi)))
            {
                return Convert.ToInt64((pi.PhysicalAvailable.ToInt64() * pi.PageSize.ToInt64() / 1048576));
            }
            else
            {
                return -1;
            }

        }

        public static Int64 GetTotalMemoryInMiB()
        {
            var pi = new PerformanceInformation();
            if (GetPerformanceInfo(out pi, Marshal.SizeOf(pi)))
            {
                return Convert.ToInt64((pi.PhysicalTotal.ToInt64() * pi.PageSize.ToInt64() / 1048576));
            }
            else
            {
                return -1;
            }

        }
    }
}
