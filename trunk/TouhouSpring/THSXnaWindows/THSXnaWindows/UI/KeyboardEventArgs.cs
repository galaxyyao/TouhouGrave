using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.UI
{
    class KeyboardEventArgs : EventArgs
    {
        public KeyboardState KeyboardState
        {
            get; private set;
        }

        public KeyboardEventArgs(KeyboardState keyboardState)
        {
            KeyboardState = keyboardState;
        }
    }

    class KeyPressedEventArgs : KeyboardEventArgs
    {
        public IIndexable<bool> KeyPressed
        {
            get; private set;
        }

        public KeyPressedEventArgs(KeyboardState keyboardState, bool[] pressedKeys)
            : base(keyboardState)
        {
            KeyPressed = pressedKeys.ToIndexable();
        }
    }

    class KeyReleasedEventArgs : KeyboardEventArgs
    {
        public IIndexable<bool> KeyReleased
        {
            get; private set;
        }

        public KeyReleasedEventArgs(KeyboardState keyboardState, bool[] releasedKeys)
            : base(keyboardState)
        {
            KeyReleased = releasedKeys.ToIndexable();
        }
    }
}
