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
        private char? m_passwordChar = null;
        private int m_caretPosition = 0;
        private float m_caretBlinkTimer = 0;
        private int m_selectionLength = 0;
        private float m_scrollPosition = 0;
        private TextRenderer.IFormattedText m_allText;

        private Animation.CurveTrack m_scrollSlideTrack;
        private Action<float> m_onTrackElapsed;
        private float m_lastScrollPosition;
        private float m_nextScrollPosition;

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
                    TextChanged();
                }
            }
        }

        public char? PasswordChar
        {
            get { return m_passwordChar; }
            set
            {
                if (value != m_passwordChar)
                {
                    m_passwordChar = value;
                    TextChanged();
                }
            }
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

        public Curve SlidingCurve
        {
            get { return m_scrollSlideTrack != null ? m_scrollSlideTrack.Curve : null; }
            set
            {
                if (value != SlidingCurve)
                {
                    if (m_scrollSlideTrack != null && m_scrollSlideTrack.IsPlaying)
                    {
                        m_scrollSlideTrack.Stop();
                        m_scrollSlideTrack = null;
                        m_scrollPosition = m_nextScrollPosition;
                    }

                    if (value != null)
                    {
                        m_scrollSlideTrack = new Animation.CurveTrack(value);
                        m_scrollSlideTrack.Elapsed += m_onTrackElapsed;
                    }
                }
            }
        }

        public TextBox(int width, int height, TextRenderer.FormatOptions textFormatOptions)
        {
            m_focusableProxy = new FocusableProxy(this);
            m_textFormatOptions = textFormatOptions;
            m_textFormatOptions.DisableRTF = true;

            m_renderableProxy = new RenderableProxy(this);

            ForeColor = Color.White;
            SelectionBackColor = Color.Navy;
            SelectionForeColor = Color.White;

            Width = width;
            Height = height;
            Region = new Rectangle(0, 0, Width, Height);

            m_onTrackElapsed = w => m_scrollPosition = MathHelper.Lerp(m_lastScrollPosition, m_nextScrollPosition, w);

            TextChanged();
        }

        public void OnRender(RenderEventArgs e)
        {
            if (m_scrollSlideTrack != null && m_scrollSlideTrack.IsPlaying)
            {
                m_scrollSlideTrack.Elapse((float)GameApp.Instance.TargetElapsedTime.TotalSeconds);
            }

            var transform = TransformToGlobal;
            var drawOptions = TextRenderer.DrawOptions.Default;
            drawOptions.DrawFlags |= TextRenderer.DrawFlags.BoundedByBox;
            drawOptions.BoundingBox = Region;
            drawOptions.ColorScaling = ForeColor.ToVector4();
            drawOptions.Offset.X = -m_scrollPosition;

            if (m_selectionLength != 0)
            {
                var selectionBegin = m_selectionLength < 0 ? m_caretPosition + m_selectionLength : m_caretPosition;
                var selectionEnd = m_selectionLength > 0 ? m_caretPosition + m_selectionLength : m_caretPosition;

                // selection background
                var selectionLeft = m_allText.MeasureWidth(0, selectionBegin) - m_scrollPosition;
                var selectionWidth = m_allText.MeasureWidth(selectionBegin, selectionEnd);

                // clamp selection into window
                if (selectionLeft < 0)
                {
                    selectionWidth += selectionLeft;
                    selectionLeft = 0;
                }
                if (selectionLeft + selectionWidth > Width)
                {
                    selectionWidth = Width - selectionLeft;
                }
                e.RenderManager.Draw(new Graphics.TexturedQuad
                {
                    ColorToModulate = SelectionBackColor
                }, new Rectangle(selectionLeft, 0, selectionWidth, Height), transform);

                // draw the text before selection
                if (selectionBegin > 0)
                {
                    drawOptions.SubstringLength = selectionBegin;
                    e.TextRenderer.DrawText(m_allText, transform, drawOptions);
                }
                // draw the selected text
                drawOptions.ColorScaling = SelectionForeColor.ToVector4();
                drawOptions.SubstringStart = selectionBegin;
                drawOptions.SubstringLength = selectionEnd - selectionBegin;
                e.TextRenderer.DrawText(m_allText, transform, drawOptions);
                // draw the text after selection
                if (selectionEnd < m_text.Length)
                {
                    drawOptions.ColorScaling = ForeColor.ToVector4();
                    drawOptions.SubstringStart = selectionEnd;
                    drawOptions.SubstringLength = m_text.Length - selectionEnd;
                    e.TextRenderer.DrawText(m_allText, transform, drawOptions);
                }
            }
            else
            {
                e.TextRenderer.DrawText(m_allText, transform, drawOptions);
            }

            // caret
            m_caretBlinkTimer += GameApp.Instance.TargetElapsedTime.Milliseconds;
            if (m_isFocused && ((int)Math.Floor(m_caretBlinkTimer / CaretBlinkTime) % 2) == 0)
            {
                var caretPosition = m_allText.MeasureWidth(0, m_caretPosition) - m_scrollPosition;
                caretPosition = MathHelper.Clamp(caretPosition, 0, Width);
                e.RenderManager.Draw(new Graphics.TexturedQuad { ColorToModulate = ForeColor },
                    new Rectangle(caretPosition - 1, 0, 2, Height), transform);
            }
        }

        public void OnGotFocus()
        {
            m_isFocused = true;
            m_caretPosition = m_text.Length;
            m_selectionLength = -m_caretPosition;
            MakeVisible();
        }

        public void OnLostFocus()
        {
            m_caretPosition = 0;
            m_selectionLength = 0;
            MakeVisible();
            m_isFocused = false;
        }

        public void OnFocusedKeyPressed(KeyPressedEventArgs e)
        {
            bool shiftPressed = e.KeyboardState.KeyPressed[(int)Keys.LeftShift]
                || e.KeyboardState.KeyPressed[(int)Keys.RightShift];
            var oldText = m_text.ToString();

            if (e.KeyPressed[(int)Keys.Left])
            {
                if (m_caretPosition == 0)
                {
                    m_selectionLength = shiftPressed ? m_selectionLength : 0;
                }
                else
                {
                    m_selectionLength = shiftPressed ? m_selectionLength + 1 : 0;
                    --m_caretPosition;
                }
            }

            if (e.KeyPressed[(int)Keys.Right])
            {
                if (m_caretPosition == m_text.Length)
                {
                    m_selectionLength = shiftPressed ? m_selectionLength : 0;
                }
                else
                {
                    m_selectionLength = shiftPressed ? m_selectionLength - 1 : 0;
                    ++m_caretPosition;
                }
            }

            if (e.KeyPressed[(int)Keys.Home])
            {
                m_selectionLength = shiftPressed ? m_caretPosition : 0;
                m_caretPosition = 0;
            }

            if (e.KeyPressed[(int)Keys.End])
            {
                m_selectionLength = shiftPressed ? m_caretPosition - m_text.Length: 0;
                m_caretPosition = m_text.Length;
            }

            if (e.KeyPressed[(int)Keys.Back])
            {
                if (m_selectionLength != 0)
                {
                    DeleteSelection();
                }
                else if (m_caretPosition != 0)
                {
                    m_text.Remove(--m_caretPosition, 1);
                }
            }

            if (e.KeyPressed[(int)Keys.Delete])
            {
                if (m_selectionLength != 0)
                {
                    DeleteSelection();
                }
                else if (m_caretPosition < m_text.Length)
                {
                    m_text.Remove(m_caretPosition, 1);
                }
            }

            bool capslock = (PInvokes.User32.GetKeyState(0x14) & 1) != 0;
            bool numlock = (PInvokes.User32.GetKeyState(0x90) & 1) != 0;
            for (int i = 0; i < e.KeyPressed.Count; ++i)
            {
                if (e.KeyPressed[i])
                {
                    char ch = TranslateChar((Keys)i, shiftPressed, capslock, numlock);
                    if (ch != 0)
                    {
                        if (m_selectionLength != 0)
                        {
                            DeleteSelection();
                        }
                        m_text.Insert(m_caretPosition++, ch);
                    }
                }
            }

            if (oldText != m_text.ToString())
            {
                TextChanged();
            }
            else
            {
                MakeVisible();
            }
            m_caretBlinkTimer = 0;
        }

        public void OnFocusedKeyReleased(KeyReleasedEventArgs e)
        {
        }

        private void TextChanged()
        {
            m_allText = GameApp.Service<TextRenderer>().FormatText(
                m_passwordChar != null ? new String(m_passwordChar.Value, m_text.Length) : m_text.ToString(),
                m_textFormatOptions);
            MakeVisible();
        }

        private void DeleteSelection()
        {
            m_text.Remove(Math.Min(m_caretPosition, m_caretPosition + m_selectionLength), Math.Abs(m_selectionLength));
            m_caretPosition = Math.Min(m_caretPosition, m_caretPosition + m_selectionLength);
            m_selectionLength = 0;
        }

        private void MakeVisible()
        {
            float caretOffset = m_allText.MeasureWidth(0, m_caretPosition);
            if (caretOffset < m_scrollPosition)
            {
                SetScrollPosition(Math.Max(caretOffset - Width / 3.0f, 0));
            }
            else if (caretOffset > m_scrollPosition + Width)
            {
                SetScrollPosition(Math.Max(caretOffset + Width / 3.0f - Width, 0));
            }
        }

        private void SetScrollPosition(float nextValue)
        {
            if (m_scrollSlideTrack != null)
            {
                if (m_scrollSlideTrack.IsPlaying)
                {
                    m_scrollSlideTrack.Stop();
                }

                m_lastScrollPosition = m_scrollPosition;
                m_nextScrollPosition = nextValue;
                m_scrollSlideTrack.Play();
            }
            else
            {
                m_scrollPosition = nextValue;
            }
        }
    }
}
