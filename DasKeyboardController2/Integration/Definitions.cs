using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DasKeyboardController2.Integration
{
    /// <summary>
    /// Slightly modified original definitions from Frank Nießen, thank you!
    /// </summary>
    static class Definitions
    {
        /// <summary>
        /// Keyboard layout (not all key coordinates are available).    
        /// Updated to EU layout.
        /// </summary>
        public readonly static byte[,] KeyboardMask = new byte[,]{
        //   00 01 02 03 04 05 06 07 08 09 10 11 12 13 14 15 16 17 18 19 20 21             
            {1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0}, //  0 
            {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0}, //  1 
            {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0}, //  2 
            {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 0, 0, 0, 1, 1, 1, 1, 0}, //  3 
            {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 0, 1, 0, 1, 1, 1, 0, 0}, //  4
            {1, 1, 1, 0, 0, 0, 1, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 0, 0}  //  5
        };

        /// <summary>
        /// Codes for key effects. 
        /// </summary>
        public readonly static Dictionary<string, byte> Effects = new Dictionary<string, byte>{
            {"SET", 0x01 },
            {"BLINK", 0x03 },
            {"BREATHE", 0x05},
            {"COLOR_CYCLE", 0x07},
            {"RIPPLE", 0x09},
            {"INV_RIPPLE", 0x0b},
            {"BG_SET", 0x81},
            {"BG_BLINK", 0x83},
            {"BG_BREATHE", 0x85},
            {"BG_COLOR_CYCLE", 0x87},
            {"BG_RIPPLE", 0x89},
            {"BG_LASER", 0x8a},
            {"BG_INV_RIPPLE", 0x8b},
            {"EFFECT_SET", 0x02},
            {"EFFECT_BLINK", 0x04},
            {"EFFECT_BREATHE", 0x06},
            {"EFFECT_COLOR_CYCLE", 0x08},
            {"EFFECT_COLOR_CLOUD", 0x0c},
        };

        /// <summary>
        /// Names of the keys to corresponding coordinates.
        /// Original (prebably German) layout.
        /// </summary>
        public readonly static Dictionary<string, string> KeyNames = new Dictionary<string, string>(){
            {"KEY_ESCAPE", "0,0"},
            {"KEY_F1", "2,0"},
            {"KEY_F2", "3,0"},
            {"KEY_F3", "4,0"},
            {"KEY_F4", "5,0"},
            {"KEY_F5", "7,0"},
            {"KEY_F6", "8,0"},
            {"KEY_F7", "9,0"},
            {"KEY_F8", "10,0"},
            {"KEY_F9", "11,0"},
            {"KEY_F10", "12,0"},
            {"KEY_F11", "13,0"},
            {"KEY_F12", "14,0"},
            {"KEY_PRINT_SCREEN", "15,0"},
            {"KEY_SCROLL_LOCK", "16,0"},
            {"KEY_PAUSE_BREAK", "17,0"},
            {"KEY_MEDIA_KEYS", "18,0"},
            {"KEY_ROOFTOP", "0,1"},
            {"KEY_1", "1,1"},
            {"KEY_2", "2,1"},
            {"KEY_3", "3,1"},
            {"KEY_4", "4,1"},
            {"KEY_5", "5,1"},
            {"KEY_6", "6,1"},
            {"KEY_7", "7,1"},
            {"KEY_8", "8,1"},
            {"KEY_9", "9,1"},
            {"KEY_0", "10,1"},
            {"KEY_ESZETT", "11,1"},
            {"KEY_ACUTE_ACCENT", "12,1"},
            {"KEY_BACKSPACE", "13,1"},
            {"KEY_INSERT", "15,1"},
            {"KEY_HOME", "16,1"},
            {"KEY_PAGE_UP", "17,1"},
            {"KEY_NUM_LOCK", "18,1"},
            {"KEY_NUMPAD_DIVIDE", "19,1"},
            {"KEY_NUMPAD_MULTIPLY", "20,1"},
            {"KEY_NUMPAD_SUBTRACT", "21,1"},
            {"KEY_TAB", "0,2"},
            {"KEY_Q", "1,2"},
            {"KEY_W", "2,2"},
            {"KEY_E", "3,2"},
            {"KEY_R", "4,2"},
            {"KEY_T", "5,2"},
            {"KEY_Z", "6,2"},
            {"KEY_U", "7,2"},
            {"KEY_I", "8,2"},
            {"KEY_O", "9,2"},
            {"KEY_P", "10,2"},
            {"KEY_UBERMUT", "11,2"},
            {"KEY_CLOSE_SQUARE_BRACKET", "12,2"},
            {"KEY_ENTER", "14,2"},
            {"KEY_DELETE", "15,2"},
            {"KEY_END", "16,2"},
            {"KEY_PAGE_DOWN", "17,2"},
            {"KEY_NUMPAD_7", "18,2"},
            {"KEY_NUMPAD_8", "19,2"},
            {"KEY_NUMPAD_9", "20,2"},
            {"KEY_NUMPAD_ADD", "21,2"},
            {"KEY_CAPS_LOCK", "0,3"},
            {"KEY_A", "2,3"},
            {"KEY_S", "3,3"},
            {"KEY_D", "4,3"},
            {"KEY_F", "5,3"},
            {"KEY_G", "6,3"},
            {"KEY_H", "7,3"},
            {"KEY_J", "8,3"},
            {"KEY_K", "9,3"},
            {"KEY_L", "10,3"},
            {"KEY_OKONOM", "11,3"},
            {"KEY_ARGER", "12,3"},
            {"KEY_HASH", "13,3"},
            {"KEY_NUMPAD_4", "18,3"},
            {"KEY_NUMPAD_5", "19,3"},
            {"KEY_NUMPAD_6", "20,3"},
            {"KEY_SHIFT_LEFT", "0,4"},
            {"KEY_LESS_THAN", "1,4"},
            {"KEY_Y", "2,4"},
            {"KEY_X", "3,4"},
            {"KEY_C", "4,4"},
            {"KEY_V", "5,4"},
            {"KEY_B", "6,4"},
            {"KEY_N", "7,4"},
            {"KEY_M", "8,4"},
            {"KEY_COMMA", "9,4"},
            {"KEY_DOT", "10,4"},
            {"KEY_SUBTRACT", "11,4"},
            {"KEY_SHIFT_RIGHT", "12,4"},
            {"KEY_ARROW_UP", "16,4"},
            {"KEY_NUMPAD_1", "18,4"},
            {"KEY_NUMPAD_2", "19,4"},
            {"KEY_NUMPAD_3", "20,4"},
            {"KEY_NUMPAD_ENTER", "21,4"},
            {"KEY_CONTROL_LEFT", "0,5"},
            {"KEY_META_LEFT", "1,5"},
            {"KEY_ALT_LEFT", "3,5"},
            {"KEY_SPACE", "4,5"},
            {"KEY_ALT_RIGHT", "10,5"},
            {"KEY_META_RIGHT", "11,5"},
            {"KEY_CONTEXT_MENU", "12,5"},
            {"KEY_CONTROL_RIGHT", "14,5"},
            {"KEY_ARROW_LEFT", "15,5"},
            {"KEY_ARROW_DOWN", "16,5"},
            {"KEY_ARROW_RIGHT", "17,5"},
            {"KEY_NUMPAD_0", "19,5"},
            {"KEY_NUMPAD_DECIMAL", "20,5"},
            {"KEY_SLEEP", "18,-1"}
        };

        /// <summary>
        /// Ids of all existing keys.
        /// </summary>
        public static readonly IEnumerable<byte> AllKeyIds;

        /// <summary>
        /// Maps coordinates to corresponding ids.
        /// </summary>
        public static readonly Dictionary<Tuple<int, int>, byte> CoordinateIds = new Dictionary<Tuple<int, int>, byte>();

        static Definitions()
        {
            var keyIds = new List<byte>();
            byte currentId = 0;
            for (var x = 0; x < KeyboardMask.GetLength(1); ++x)
            {
                for (var y = KeyboardMask.GetLength(0) - 1; y >= 0; --y)
                {
                    var maskValue = KeyboardMask[y, x];
                    if (maskValue > 0)
                    {
                        keyIds.Add(currentId);
                        CoordinateIds[Tuple.Create(x, y)] = currentId;
                    }

                    currentId += 1;
                }
            }

            keyIds.Add(126); // sleep
            keyIds.Add(127); // brightness
            keyIds.Add(128); // play
            keyIds.Add(129); // next chapter

            AllKeyIds = keyIds.Distinct().ToArray();
        }
    }
}
