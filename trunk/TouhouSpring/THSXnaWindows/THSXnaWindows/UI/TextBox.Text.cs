using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TouhouSpring.Graphics;

namespace TouhouSpring.UI
{
    partial class TextBox : ITextReceiver
    {
        private bool m_inComposition;
        private TextRenderer.IFormattedText m_compositionString;
        private int m_compositionCursorPos;

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

        void ITextReceiver.OnInputLanguageChange(string lang)
        {
            m_inidcatorStr = GameApp.Service<Graphics.TextRenderer>().FormatText(lang, m_textFormatOptions);
        }

        void ITextReceiver.OnStartComposition()
        {
        }

        void ITextReceiver.OnComposition(string compositionString, int cursorPos)
        {
            System.Diagnostics.Trace.WriteLine(String.Format("Composition: {0} {1}/{2}", compositionString, cursorPos, m_compositionCursorPos));
            m_inComposition = !String.IsNullOrEmpty(compositionString);

            if (m_compositionString == null || m_compositionString.Text != compositionString)
            {
                m_compositionString = GameApp.Service<TextRenderer>().FormatText(compositionString, m_textFormatOptions);
            }

            m_compositionCursorPos = cursorPos;
            MakeVisible();
        }

        void ITextReceiver.OnEndComposition()
        {
            m_inComposition = false;
            m_compositionCursorPos = 0;
            MakeVisible();
        }
    }
}
