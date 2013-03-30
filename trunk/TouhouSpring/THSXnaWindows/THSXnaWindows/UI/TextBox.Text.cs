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

        void ITextReceiver.OnChar(char code)
        {
            if (m_selectionLength != 0)
            {
                DeleteSelection();
            }
            m_text.Insert(m_caretPosition++, code);

            TextChanged();
            m_caretBlinkTimer = 0;
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
