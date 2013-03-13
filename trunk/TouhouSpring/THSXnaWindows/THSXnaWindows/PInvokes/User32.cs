using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace TouhouSpring.PInvokes
{
    public class User32
    {
        [DllImport("user32.dll")]
        public static extern uint GetCaretBlinkTime();

        [DllImport("user32.dll")]
        public static extern ushort GetKeyState(int keyCode);
    }
}
