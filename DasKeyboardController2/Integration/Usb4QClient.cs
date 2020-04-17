using DK4Q.Integration;
using HidLibrary;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DasKeyboardController2.Integration
{
    class Usb4QDriver
    {
        private readonly static int PACKET_SIZE = 8;

        private readonly static int VENDOR_ID = 0x24f0;

        private readonly static int PRODUCT_ID = 0x2037;

        private readonly static int USAGE_ID = 128;

        private readonly bool _verbose;

        private HidDevice _leds;

        private byte[] _buffer;

        private byte _currentChecksum;


        public Usb4QDriver(bool verbose = false)
        {
            _verbose = verbose;
            setupHid();
        }

        #region Driver API

        internal void WaitForConnection()
        {
            while (_leds == null)
            {
                setupHid();
                Thread.Sleep(5000);
            }
        }

        internal void SetAllKeys(Color color)
        {
            foreach (var keyId in Definitions.AllKeyIds)
            {
                WriteToDeviceBuffer(keyId, color, KeyState.SET_COLOR);
            }
        }

        internal void ForgetKeys()
        {
            //TODO implement
        }

        internal void SetKey(int x, int y, Color color, string effect)
        {
            var coord = Tuple.Create(x, y);
            if (!Definitions.CoordinateIds.TryGetValue(coord, out var keyId))
                return; // invalid coordinate will be skipped

            WriteToDeviceBuffer(keyId, color, effect);
        }

        #endregion

        #region Direct API

        internal void WriteToDeviceBuffer(int k, Color color, string effect)
        {
            var c = color;

            send($"01 EA 0B 78 03 {k:X2} 00 00", resetChecksum: true);
            send("01 00 00 00 00 00 $S 00");
            readToBuffer();

            send($"01 EA 08 78 08 {k:X2} 01 {c.R:X2}", resetChecksum: true);
            send($"01 {c.G:X2} {c.B:X2} $S 00 00 00 00");

            send($"01 EA 0B 78 04 {k:X2} 00 00", resetChecksum: true);
            send("01 00 00 00 00 00 $S 00");
            readToBuffer();
            //expectEmptyBuffer();
        }

        public void Flush()
        {
            // make the buffer visible
            send("01 EA 03 78 0A 9B 00 00");
            Thread.Sleep(300);
            readAndExpect("ED 03 78 00 96 00 00 00");
            readAll(expectEmpty: true);
        }

        public void Close()
        {
            if (_leds != null)
                _leds.CloseDevice();
        }


        #endregion

        #region HID control

        private void setupHid()
        {
            if (_leds != null && _leds.IsConnected)
            {
                // there is nothing to do 
                return;
            }

            _leds = _findLedsHid();
            if (_leds == null)
            {
                // not connected
                return;
            }

            _leds.OpenDevice();
            if (!_leds.IsConnected || !_leds.IsOpen)
            {
                throw new NotImplementedException();
            }
        }

        private HidDevice _findLedsHid()
        {
            foreach (var device in HidDevices.Enumerate(VENDOR_ID, new int[] { PRODUCT_ID }))
            {
                if (device.Capabilities.Usage == USAGE_ID)
                {
                    return device;
                }
            }

            // device was not found
            return null;
        }

        private void send(string dataStr, bool resetChecksum = false)
        {
            if (resetChecksum)
            {
                _currentChecksum = 0;
            }

            var data = convertStrToBytes(dataStr, ref _currentChecksum);
            send(data);
        }

        private void send(byte[] data)
        {
            if (data.Length != PACKET_SIZE)
            {
                throw new ArgumentException("Incorrect packet size");
            }

            log("SEND: " + convertBytesToStr(data));

            var report = _leds.CreateReport();
            report.Data = data;
            report.ReportId = 1;
            var result = _leds.WriteFeatureData(data);
            if (result)
            {
                log("\t " + result);
            }
            else
            {
                error("SENDING: " + convertBytesToStr(data));

                reportDisconnection();
            }
        }

        private void reportDisconnection()
        {
            if (_leds != null)
            {
                _leds.CloseDevice();
                _leds = null;
            }
        }

        private void readAndExpect(string expectedValueStr)
        {
            readToBuffer();

            byte checksum = 0;
            var expectedValue = convertStrToBytes(expectedValueStr, ref checksum);
            if (!expectedValue.SequenceEqual(_buffer))
            {
                error($"Expected {expectedValueStr} and got {convertBytesToStr(_buffer)}");
            }
        }

        private void readToBuffer()
        {
            _leds.ReadFeatureData(out _buffer, reportId: 1);

            log("READ: " + convertBytesToStr(_buffer));
        }

        private void expectEmptyBuffer()
        {
            var sum = _buffer.Sum(b => b);
            if (sum != 0)
            {
                error("EXPECTED EMPTY: " + convertBytesToStr(_buffer));
            }
        }

        private int readAll(bool expectEmpty = false)
        {
            var count = 0;
            for (; ; )
            {
                count += 1;
                readToBuffer();
                if (expectEmpty)
                {
                    expectEmptyBuffer();
                }

                if (_buffer == null)
                {
                    break;
                }

                var acc = 0;
                foreach (var b in _buffer)
                {
                    acc += b;
                }
                if (acc == 0)
                    break;
            }

            return count;
        }

        private byte[] convertStrToBytes(string dataStr, ref byte currentChecksum)
        {
            var tokens = dataStr.Trim().Split(' ');
            var bytes = new byte[tokens.Length];
            for (var i = 0; i < bytes.Length; ++i)
            {
                var token = tokens[i];
                if (token == "$S")
                {
                    bytes[i] = currentChecksum;
                }
                else
                {
                    var tokenByte = Convert.ToByte(token, 16);
                    bytes[i] = tokenByte;
                    currentChecksum ^= tokenByte;
                }
            }

            return bytes;
        }

        private string convertBytesToStr(byte[] bytes)
        {
            if (bytes == null)
            {
                return "null";
            }

            var result = new StringBuilder();
            foreach (var b in bytes)
            {
                if (result.Length > 0)
                    result.Append(" ");

                result.Append(b.ToString("X2"));
            }

            return result.ToString();
        }

        #endregion

        #region Logging

        private void log(string message)
        {
            if (_verbose)
            {
                Console.WriteLine($"{DateTime.Now} {message}");
            }
        }

        private void error(string message)
        {
            Console.WriteLine($"{DateTime.Now} [ERROR] {message}");
        }

        #endregion
    }
}
