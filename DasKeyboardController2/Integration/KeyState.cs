using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace DK4Q.Integration
{
    class KeyState
    {
        internal const string SET_COLOR = "SET_COLOR";

        internal const string BLINK = "BLINK";

        internal const string BREATHE = "BREATHE";

        internal Color Color;

        internal string Effect = SET_COLOR;

        internal void UpdateBy(KeyState other)
        {
            Color = other.Color;
            Effect = other.Effect;
        }

        internal bool IsSameAs(KeyState other)
        {
            return Color == other.Color && Effect == other.Effect;
        }

        internal KeyState CreateSnapshot()
        {
            var snapshot = new KeyState();

            snapshot.Color = Color;
            snapshot.Effect = Effect;

            return snapshot;
        }
    }
}
