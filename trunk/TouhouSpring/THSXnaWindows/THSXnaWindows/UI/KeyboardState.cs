using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XnaInput = Microsoft.Xna.Framework.Input;

namespace TouhouSpring.UI
{
    class KeyboardState
    {
        private const int NumKeys = 256;
        private bool[] m_keyPressed = new bool[NumKeys];

        public IIndexable<bool> KeyPressed
        {
            get; private set;
        }

        public KeyboardState()
        {
            KeyPressed = m_keyPressed.ToIndexable();
        }

        public KeyboardState(XnaInput.KeyboardState xnaKeyboardState)
            : this()
        {
            for (int i = 0; i < NumKeys; ++i)
            {
                m_keyPressed[i] = xnaKeyboardState[(XnaInput.Keys)i] == XnaInput.KeyState.Down;
            }
        }

        public static void Differentiate(KeyboardState from, KeyboardState to,
            out bool[] pressedKeys, out bool[] releasedKeys)
        {
            pressedKeys = new bool[NumKeys];
            releasedKeys = new bool[NumKeys];
            for (int i = 0; i < NumKeys; ++i)
            {
                pressedKeys[i] = !from.KeyPressed[i] && to.KeyPressed[i];
                releasedKeys[i] = from.KeyPressed[i] && !to.KeyPressed[i];
            }
        }
    }
}
