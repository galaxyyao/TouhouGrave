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
        public char KeyPressed
        {
            get; private set;
        }

        public KeyPressedEventArgs(KeyboardState keyboardState, char keyPressed)
            : base(keyboardState)
        {
            KeyPressed = keyPressed;
        }
    }

    class KeyReleasedEventArgs : KeyboardEventArgs
    {
        public char KeyReleased
        {
            get; private set;
        }

        public KeyReleasedEventArgs(KeyboardState keyboardState, char keyReleased)
            : base(keyboardState)
        {
            KeyReleased = keyReleased;
        }
    }
}
