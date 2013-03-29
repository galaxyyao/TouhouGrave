using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.UI
{
    partial class TextBox : ITextReceiver
    {
        public bool ImeEnabled
        {
            get; private set;
        }

        void ITextReceiver.OnChar()
        {
        }

        void ITextReceiver.OnInputLanguageChange()
        {
        }

        void ITextReceiver.OnStartComposition()
        {
        }

        void ITextReceiver.OnComposition()
        {
        }

        void ITextReceiver.OnEndComposition()
        {
        }
    }
}
