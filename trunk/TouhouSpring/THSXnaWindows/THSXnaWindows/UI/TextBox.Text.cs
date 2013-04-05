using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TouhouSpring.Graphics;
using Color = Microsoft.Xna.Framework.Color;

namespace TouhouSpring.UI
{
    partial class TextBox : ITextReceiver
    {
        private class CandidateListLabel : Label
        {
            public TextBox m_textBox;

            public override void OnRender(RenderEventArgs e)
            {
                if (FormattedText != null)
                {
                    var transform = TransformToGlobal;
                    e.RenderManager.Draw(new TexturedQuad { ColorToModulate = m_textBox.ImeCandidateListBackColor },
                        new Rectangle(0, 0, FormattedText.Size.Width, FormattedText.Size.Height),
                        transform);
                    var selection = FormattedText.GetLineRectangle(m_textBox.m_candidateListData.Selection);
                    e.RenderManager.Draw(new TexturedQuad { ColorToModulate = m_textBox.ImeCandidateListSelectionBackColor },
                        selection, transform);
                }

                base.OnRender(e);
            }
        }

        private bool m_inComposition;
        private TextRenderer.IFormattedText m_compositionString;
        private Ime.ClauseAttribute[] m_compStrAttr;
        private int m_compositionCursorPos;

        private Ime.CandidateListData m_candidateListData;
        private TextRenderer.IFormattedText m_candidateListText;
        private CandidateListLabel m_candidateListLabel;

        public Color ImeCompositionStringBackColor
        {
            get; set;
        }

        public Color ImeCompositionStringForeColor
        {
            get; set;
        }

        public Color ImeCandidateListBackColor
        {
            get; set;
        }

        public Color ImeCandidateListForeColor
        {
            get; set;
        }

        public Color ImeCandidateListSelectionBackColor
        {
            get; set;
        }

        public Color ImeCandidateListSelectionForeColor
        {
            get; set;
        }

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

        void ITextReceiver.OnComposition(string compositionString, Ime.ClauseAttribute[] attr, int cursorPos)
        {
            m_inComposition = !String.IsNullOrEmpty(compositionString);

            if (m_compositionString == null || m_compositionString.Text != compositionString)
            {
                m_compositionString = GameApp.Service<TextRenderer>().FormatText(compositionString, m_textFormatOptions);
            }
            m_compStrAttr = attr;
            m_compositionCursorPos = cursorPos;

            MakeVisible();
        }

        void ITextReceiver.OnEndComposition()
        {
            m_inComposition = false;
            m_compositionCursorPos = 0;
            MakeVisible();
        }

        void ITextReceiver.OnCandidateListUpdate(Ime.CandidateListData data)
        {
            m_candidateListData = data;
            if (m_candidateListData.IsOpened)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("[color:!");
                sb.Append(ImeCandidateListForeColor.ToHex(true));
                sb.Append("]");
                for (int i = 0; i < data.Candidates.Length; ++i)
                {
                    if (i == data.Selection)
                    {
                        sb.Append("[color:!");
                        sb.Append(ImeCandidateListSelectionForeColor.ToHex(true));
                        sb.Append("]");
                    }
                    sb.Append(data.Candidates[i]);
                    if (i == data.Selection)
                    {
                        sb.Append("[/color]");
                    }
                    sb.Append("\n");
                }
                sb.Append(">");
                sb.Append(data.PageIndex + 1);
                sb.Append("/");
                sb.Append(data.PageCount);
                sb.Append("[/color]");

                // get the target clause
                var candTarget = m_compStrAttr.FindIndex(attr => attr == Ime.ClauseAttribute.TargetConverted || attr == Ime.ClauseAttribute.TargetNotConverted);
                var candListLeft = m_allText.MeasureLeft(m_caretPosition) + m_compositionString.MeasureLeft(candTarget) - m_scrollPosition;

                m_candidateListText = GameApp.Service<TextRenderer>().FormatText(sb.ToString(), m_candidateListFormatOptions);
                m_candidateListLabel.FormattedText = m_candidateListText;

                m_candidateListLabel.Transform = MatrixHelper.Translate(candListLeft, Height) * Transform;
                m_candidateListLabel.Dispatcher = Dispatcher;
            }
            else
            {
                m_candidateListText = null;
                m_candidateListLabel.FormattedText = null;
                m_candidateListLabel.Dispatcher = null;
            }
        }
    }
}
