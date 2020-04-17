using System;
using System.Drawing;
using System.Threading;

using DK4Q.InfoProviders;
using DK4Q.Integration;
using System.Globalization;

using DasKeyboardController2.Integration;
using DasKeyboardController2.InfoProviders;

namespace DK4Q
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

            var usbDriver = new Usb4QDriver(verbose: false);
            // var qToolsDriver = new QToolsDriver(27301);

            var client = new RgbClient(usbDriver);
            // runUsbDemo(usbDriver);
            try
            {
                startupSequence(client);
            }
            catch (Exception)
            {
                Console.WriteLine("STARTUP SEQUENCE WAS SKIPPED");
            }
            renderProviders(client);
        }

        static void renderProviders(RgbClient client)
        {
            var cpuProvider = new CpuInfoProvider();
            var ramProvider = new RamInfoProvider();
            var pingProvider = new PingInfoProvider("8.8.8.8");
            var extendedPingProvider = new ExtendedPingInfoProvider("8.8.8.8");

            var lastRefresh = DateTime.MinValue;
            var wasDisconnected = true;

            while (true)
            {
                try
                {
                    var currentRefreshTime = DateTime.Now;
                    if (wasDisconnected || (currentRefreshTime - lastRefresh).TotalSeconds > 30)
                    {
                        // refresh background (e.g. when waking up)
                        Thread.Sleep(1000); // prevent any race conditions

                        client.ForgetKeys();
                        client.SetAllKeys(Color.White);
                        wasDisconnected = false;
                    }

                    lastRefresh = currentRefreshTime;

                    cpuProvider.Render(client);
                    ramProvider.Render(client);
                    pingProvider.Render(client);
                    extendedPingProvider.Render(client);
                    client.Flush();
                    Thread.Sleep(200);
                }
                catch (Exception)
                {
                    Console.WriteLine("DISCONNECTED");
                    client.WaitForConnection();
                    wasDisconnected = true;
                    Console.WriteLine("CONNECTED AGAIN");
                }
            }
        }

        static void startupSequence(RgbClient client)
        {
            var sequenceColors = new Color[] { Color.Red, Color.Green, Color.Blue, Color.Yellow, Color.White };
            foreach (var color in sequenceColors)
            {
                client.SetAllKeys(color);
                client.Flush();
                Thread.Sleep(200);
            }
        }

        private static void runUsbDemo(Usb4QDriver usbClient)
        {
            try
            {
                var rnd = new Random();
                var j = 0;

                while (true)
                {
                    foreach (var i in Definitions.AllKeyIds)
                    {
                        var color = QToolsDriver.ColorFromHSV(rnd.NextDouble() * 360, 1.0, 1.0);
                        // color = KeyboardClient.ColorFromHSV(j * 60 + i * 5, 1.0, 1.0);
                        usbClient.WriteToDeviceBuffer(i, color, KeyState.SET_COLOR);
                    }

                    usbClient.Flush();
                    j += 1;
                }
                //usbCLient.PrintCheckSums();
            }
            finally
            {
                usbClient.Close();
            }
        }
    }
}
