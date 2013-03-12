using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using TouhouSpring.Graphics;

namespace TouhouSpring.UI
{
    partial class TextBox : MouseTrackedControl, IRenderable, IFocusable
    {
        private bool m_isFocused = false;
        private FocusableProxy m_focusableProxy;
        private TextRenderer.FormatOptions m_textFormatOptions;

        private RenderableProxy m_renderableProxy;

        private StringBuilder m_text = new StringBuilder();
        private int m_caretPosition = 0;
        private float m_caretBlinkTimer = 0;
        private int m_scrollPosition = 0;
        private int m_scrollWindow = 0;
        private int m_selectionLength = 0;
        private TextRenderer.IFormattedText m_allText;

        private static float CaretBlinkTime = PInvokes.User32.GetCaretBlinkTime();

        public string Text
        {
            get { return m_text.ToString(); }
            set
            {
                var newValue = value ?? String.Empty;
                if (m_text.ToString() != newValue)
                {
                    m_text.Clear();
                    m_text.Append(newValue);
                    OnTextChanged();
                }
            }
        }

        public Color BackColor
        {
            get; set;
        }

        public Color ForeColor
        {
            get; set;
        }

        public Color SelectionBackColor
        {
            get; set;
        }

        public Color SelectionForeColor
        {
            get; set;
        }

        public int Width
        {
            get; private set;
        }

        public int Height
        {
            get; private set;
        }

        public TextBox(int width, int height, TextRenderer.FormatOptions textFormatOptions)
        {
            m_focusableProxy = new FocusableProxy(this);
            m_textFormatOptions = textFormatOptions;

            m_renderableProxy = new RenderableProxy(this);

            BackColor = Color.Black;
            ForeColor = Color.White;
            SelectionBackColor = Color.Navy;
            SelectionForeColor = Color.White;

            Width = width;
            Height = height;
            Region = new Rectangle(0, 0, Width, Height);

            OnTextChanged();
        }

        public void OnRender(RenderEventArgs e)
        {
            var transform = TransformToGlobal;
            var caretPosition = m_allText.MeasureWidth(m_scrollPosition, m_caretPosition);
            var drawOptions = TextRenderer.DrawOptions.Default;
            drawOptions.ColorScaling = ForeColor.ToVector4();

            // background
            e.RenderManager.Draw(new Graphics.TexturedQuad
            {
                ColorToModulate = BackColor
            }, Region, transform);

            //if (m_selectionLength > 0)
            //{
            //    var selectionWidth = m_allText.MeasureWidth(m_caretPosition,
            //        Math.Min(m_caretPosition + m_selectionLength, m_scrollPosition + m_scrollWindow));

            //    // selection background
            //    e.RenderManager.Draw(new Graphics.TexturedQuad
            //    {
            //        ColorToModulate = SelectionBackColor
            //    }, new Rectangle(caretPosition, 0, selectionWidth, Height), transform);

            //    // draw the text before selection
            //    if (m_caretPosition > m_scrollPosition)
            //    {
            //        drawOptions.SubstringStart = m_scrollPosition;
            //        drawOptions.SubstringLength = m_caretPosition - m_scrollPosition;
            //        e.TextRenderer.DrawText(m_allText, transform, drawOptions);
            //    }
            //    // draw the selected text
            //    drawOptions.SubstringStart = m_caretPosition;
            //    drawOptions.SubstringLength = m_selectionLength;
            //    // draw the text after selection
            //}
            //else
            {
                // text
                drawOptions.SubstringStart = m_scrollPosition;
                drawOptions.SubstringLength = m_scrollWindow;
                e.TextRenderer.DrawText(m_allText, transform, drawOptions);
            }

            // caret
            m_caretBlinkTimer += GameApp.Instance.TargetElapsedTime.Milliseconds;
            if (m_isFocused && ((int)Math.Floor(m_caretBlinkTimer / CaretBlinkTime) % 2) == 0)
            {
                e.RenderManager.Draw(new Graphics.TexturedQuad { ColorToModulate = ForeColor },
                    new Rectangle(caretPosition - 1, 0, 2, Height), transform);
            }
        }

        public void OnGotFocus()
        {
            m_isFocused = true;
        }

        public void OnLostFocus()
        {
            m_isFocused = false;
        }

        public void OnFocusedKeyPressed(KeyPressedEventArgs e)
        {
            if (e.KeyPressed[(int)Keys.Left])
            {
                --m_caretPosition;
                MakeVisible();
            }

            if (e.KeyPressed[(int)Keys.Right])
            {
                ++m_caretPosition;
                MakeVisible();
            }

            if (e.KeyPressed[(int)Keys.Home])
            {
                m_caretPosition = 0;
                MakeVisible();
            }

            if (e.KeyPressed[(int)Keys.End])
            {
                m_caretPosition = m_text.Length;
                MakeVisible();
            }

            if (e.KeyPressed[(int)Keys.Back]
                && m_caretPosition != 0)
            {
                m_text.Remove(--m_caretPosition, 1);
            }

            if (e.KeyPressed[(int)Keys.Delete]
                && m_caretPosition < m_text.Length)
            {
                m_text.Remove(m_caretPosition, 1);
            }

            bool shiftPressed = e.KeyboardState.KeyPressed[(int)Keys.LeftShift]
                || e.KeyboardState.KeyPressed[(int)Keys.RightShift];
            bool capslock = (PInvokes.User32.GetKeyState(0x14) & 1) != 0;
            bool numlock = (PInvokes.User32.GetKeyState(0x90) & 1) != 0;
            for (int i = 0; i < e.KeyPressed.Count; ++i)
            {
                if (e.KeyPressed[i])
                {
                    char ch = TranslateChar((Keys)i, shiftPressed, capslock, numlock);
                    if (ch != 0)
                    {
                        m_text.Insert(m_caretPosition++, ch);
                    }
                }
            }

            OnTextChanged();
            m_caretBlinkTimer = 0;
        }

        public void OnFocusedKeyReleased(KeyReleasedEventArgs e)
        {
            
        }

        private void OnTextChanged()
        {
            m_allText = GameApp.Service<TextRenderer>().FormatText(m_text.ToString(), m_textFormatOptions);
            MakeVisible();
        }

        private void MakeVisible()
        {
            if (m_caretPosition < 0)
            {
                m_caretPosition = 0;
            }
            else if (m_caretPosition > Text.Length)
            {
                m_caretPosition = Text.Length;
            }

            if (m_caretPosition < m_scrollPosition)
            {
                m_scrollPosition = m_caretPosition;
            }
            else if (m_allText.MeasureWidth(m_scrollPosition, m_caretPosition) > Width)
            {
                for (m_scrollPosition = m_caretPosition - 1;
                     m_scrollPosition >= 0 && m_allText.MeasureWidth(m_scrollPosition, m_caretPosition) <= Width;
                     --m_scrollPosition)
                    ;
                ++m_scrollPosition;
            }

            for (m_scrollWindow = 1;
                 m_scrollPosition + m_scrollWindow <= m_allText.Text.Length
                 && m_allText.MeasureWidth(m_scrollPosition, m_scrollPosition + m_scrollWindow) <= Width;
                 ++m_scrollWindow)
                ;
            --m_scrollWindow;
        }
    }
}
