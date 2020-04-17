using DK4Q.Integration;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DasKeyboardController2.Integration
{
    class RgbClient
    {
        private readonly Usb4QDriver _driver;

        public RgbClient(Usb4QDriver usbDriver)
        {
            _driver = usbDriver;
        }

        internal void SetAllKeys(Color color)
        {
            _driver.SetAllKeys(color);
        }

        internal void ForgetKeys()
        {
            _driver.ForgetKeys();
        }

        internal void SetKey(int x, int y, Color color)
        {
            SetKey(x, y, color, KeyState.SET_COLOR);
        }

        internal void SetKey(int x, int y, Color color, string effect)
        {
            _driver.SetKey(x, y, color, effect);
        }

        internal void Flush()
        {
            _driver.Flush();
        }

        internal void WaitForConnection()
        {
            _driver.WaitForConnection();
        }
    }
}
