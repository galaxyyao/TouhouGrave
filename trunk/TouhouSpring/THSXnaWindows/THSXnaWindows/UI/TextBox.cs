using System;
using System.Collections.Generic;
using System.Linq;
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
        private TextRenderer.IFormattedText m_inidcatorStr;

        private Animation.CurveTrack m_scrollSlideTrack;
        private Action<float> m_onTrackElapsed;
        private float m_lastScrollPosition;
        private float m_currentScrollPosition;

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

        public Color ImeIndicatorBackColor
        {
            get; set;
        }

        public Color ImeIndicatorForeColor
        {
            get; set;
        }

        public int ImeIndicatorWidth
        {
            get; set;
        }

        public int ImeIndicatorMargin
        {
            get; set;
        }

        public Color ImeCompositionStringBackColor
        {
            get; set;
        }

        public Color ImeCompositionStringForeColor
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

        private int InputAreaWidth
        {
            get { return m_isFocused ? Width - ImeIndicatorWidth - ImeIndicatorMargin : Width; }
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

            ImeEnabled = true;

            ForeColor = Color.White;
            SelectionBackColor = Color.Navy;
            SelectionForeColor = Color.White;
            ImeIndicatorBackColor = Color.RoyalBlue;
            ImeIndicatorForeColor = Color.White;
            ImeIndicatorWidth = height;
            ImeIndicatorMargin = 2;
            ImeCompositionStringBackColor = Color.Black;
            ImeCompositionStringForeColor = Color.White;

            Width = width;
            Height = height;
            Region = new Rectangle(0, 0, Width, Height);

            m_onTrackElapsed = w => m_currentScrollPosition = MathHelper.Lerp(m_lastScrollPosition, m_scrollPosition, w);

            TextChanged();
        }

        public void OnRender(RenderEventArgs e)
        {
            var scrollPosition = m_scrollPosition;
            if (m_scrollSlideTrack != null && m_scrollSlideTrack.IsPlaying)
            {
                m_scrollSlideTrack.Elapse((float)GameApp.Instance.TargetElapsedTime.TotalSeconds);
                scrollPosition = m_currentScrollPosition;
            }

            var transform = TransformToGlobal;
            var drawOptions = TextRenderer.DrawOptions.Default;
            drawOptions.DrawFlags |= TextRenderer.DrawFlags.BoundedByBox;
            drawOptions.BoundingBox = new Rectangle(0, 0, InputAreaWidth, Height);
            drawOptions.ColorScaling = ForeColor.ToVector4();
            drawOptions.Offset.X = -scrollPosition;

            if (m_selectionLength != 0)
            {
                var selectionBegin = m_selectionLength < 0 ? m_caretPosition + m_selectionLength : m_caretPosition;
                var selectionEnd = m_selectionLength > 0 ? m_caretPosition + m_selectionLength : m_caretPosition;

                // selection background
                var selectionLeft = m_allText.MeasureLeft(selectionBegin) - scrollPosition;
                var selectionWidth = m_allText.MeasureWidth(selectionBegin, selectionEnd);

                // clamp selection into window
                if (selectionLeft < 0)
                {
                    selectionWidth += selectionLeft;
                    selectionLeft = 0;
                }
                if (selectionLeft + selectionWidth > InputAreaWidth)
                {
                    selectionWidth = InputAreaWidth - selectionLeft;
                }

                if (selectionWidth > 0)
                {
                    e.RenderManager.Draw(new TexturedQuad
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
            }
            else
            {
                e.TextRenderer.DrawText(m_allText, transform, drawOptions);
            }

            // caret
            m_caretBlinkTimer += GameApp.Instance.TargetElapsedTime.Milliseconds;
            if (m_isFocused && !m_inComposition && ((int)Math.Floor(m_caretBlinkTimer / CaretBlinkTime) % 2) == 0)
            {
                var caretPosition = m_allText.MeasureWidth(0, m_caretPosition) - scrollPosition;
                caretPosition = MathHelper.Clamp(caretPosition, 0, InputAreaWidth);
                e.RenderManager.Draw(new TexturedQuad { ColorToModulate = ForeColor },
                    new Rectangle(caretPosition - 1, 0, 2, Height), transform);
            }

            // ime indicator
            if (m_isFocused)
            {
                e.RenderManager.Draw(new TexturedQuad { ColorToModulate = ImeIndicatorBackColor },
                    new Rectangle(Width - ImeIndicatorWidth, 0, ImeIndicatorWidth, Height), transform);
                var indicatorWidth = m_inidcatorStr.MeasureWidth(0, m_inidcatorStr.Text.Length);
                var drawOptions2 = TextRenderer.DrawOptions.Default;
                drawOptions2.ColorScaling = ImeIndicatorForeColor.ToVector4();
                drawOptions2.Offset.X = Width - ImeIndicatorWidth / 2 - indicatorWidth / 2;
                e.TextRenderer.DrawText(m_inidcatorStr, transform, drawOptions2);
            }

            // composition string
            if (m_inComposition)
            {
                // composition background
                var caretPosition = m_allText.MeasureLeft(m_caretPosition) - scrollPosition;
                var compositionLeft = caretPosition;
                var compositionWidth = m_compositionString.MeasureWidth(0, m_compositionString.Text.Length);

                // clamp composition into window
                if (compositionLeft < 0)
                {
                    compositionWidth += compositionLeft;
                    compositionLeft = 0;
                }
                if (compositionLeft + compositionWidth > InputAreaWidth)
                {
                    compositionWidth = InputAreaWidth - compositionLeft;
                }

                if (compositionWidth > 0)
                {
                    e.RenderManager.Draw(new TexturedQuad
                    {
                        ColorToModulate = ImeCompositionStringBackColor
                    }, new Rectangle(compositionLeft, 0, compositionWidth, Height), transform);

                    var drawOptions2 = TextRenderer.DrawOptions.Default;
                    drawOptions2.DrawFlags |= TextRenderer.DrawFlags.BoundedByBox;
                    drawOptions2.BoundingBox = new Rectangle(0, 0, InputAreaWidth, Height);
                    drawOptions2.ColorScaling = ImeCompositionStringForeColor.ToVector4();
                    drawOptions2.Offset.X = caretPosition;
                    e.TextRenderer.DrawText(m_compositionString, transform, drawOptions2);

                    for (int i = 0; i < m_compStrAttr.Length; ++i)
                    {
                        // get the clause range sharing the same attribute
                        Ime.ClauseAttribute clauseAttr = m_compStrAttr[i];
                        int j = i + 1;
                        for (; j < m_compStrAttr.Length; ++j)
                        {
                            if (m_compStrAttr[j] != clauseAttr)
                            {
                                break;
                            }
                        }

                        float clauseLeft = m_compositionString.MeasureLeft(i) + caretPosition;
                        float clauseWidth = m_compositionString.MeasureWidth(i, j);
                        // clamp clause into window
                        if (clauseLeft < 0)
                        {
                            clauseWidth += clauseLeft;
                            clauseLeft = 0;
                        }
                        if (clauseLeft + clauseWidth > InputAreaWidth)
                        {
                            clauseWidth = InputAreaWidth - clauseLeft;
                        }
                        if (clauseWidth > 0)
                        {
                            VirtualTexture lineTexture;
                            float lineThickness;
                            switch (clauseAttr)
                            {
                                case Ime.ClauseAttribute.Input:
                                case Ime.ClauseAttribute.InputError:
                                    lineTexture = GameApp.Service<Resources>().DashLine;
                                    lineThickness = 1f;
                                    break;
                                case Ime.ClauseAttribute.TargetConverted:
                                case Ime.ClauseAttribute.TargetNotConverted:
                                    lineTexture = GameApp.Service<Resources>().SolidLine;
                                    lineThickness = 2f;
                                    break;
                                default:
                                    lineTexture = GameApp.Service<Resources>().SolidLine;
                                    lineThickness = 1f;
                                    break;
                            }
                            
                            e.RenderManager.Draw(new TexturedQuad(lineTexture)
                            {
                                UVBounds = new Rectangle(0, 0, clauseWidth, GameApp.Service<Resources>().DashLine.Height),
                                WrapUV = true
                            }, new Rectangle(clauseLeft, Height - lineThickness - 1, clauseWidth, lineThickness), transform);
                        }

                        i = j - 1;
                    }

                    // composition caret
                    if (((int)Math.Floor(m_caretBlinkTimer / CaretBlinkTime) % 2) == 0)
                    {
                        var caretPosition2 = caretPosition + m_compositionString.MeasureWidth(0, m_compositionCursorPos);
                        caretPosition2 = MathHelper.Clamp(caretPosition2, 0, InputAreaWidth);
                        e.RenderManager.Draw(new TexturedQuad { ColorToModulate = ImeCompositionStringForeColor },
                            new Rectangle(caretPosition2 - 1, 0, 2, Height), transform);
                    }
                }
            }
        }

        public void OnGotFocus()
        {
            m_isFocused = true;
            m_caretPosition = m_text.Length;
            m_selectionLength = -m_caretPosition;
            SetScrollPosition(Math.Max(m_allText.MeasureWidth(0, m_allText.Text.Length) - InputAreaWidth, 0));
        }

        public void OnLostFocus()
        {
            m_caretPosition = 0;
            m_selectionLength = 0;
            SetScrollPosition(0);
            m_isFocused = false;
        }

        public void OnFocusedKeyPressed(KeyPressedEventArgs e)
        {
            if (m_inComposition)
            {
                // ignore all key events if IME is kicked in
                return;
            }

            bool shiftPressed = e.KeyboardState.KeyPressed[(int)Keys.LeftShift]
                || e.KeyboardState.KeyPressed[(int)Keys.RightShift];
            var oldText = m_text.ToString();

            switch (e.KeyPressed)
            {
                case (char)Keys.Left:
                    if (m_caretPosition == 0)
                    {
                        m_selectionLength = shiftPressed ? m_selectionLength : 0;
                    }
                    else
                    {
                        m_selectionLength = shiftPressed ? m_selectionLength + 1 : 0;
                        --m_caretPosition;
                    }
                    break;
                case (char)Keys.Right:
                    if (m_caretPosition == m_text.Length)
                    {
                        m_selectionLength = shiftPressed ? m_selectionLength : 0;
                    }
                    else
                    {
                        m_selectionLength = shiftPressed ? m_selectionLength - 1 : 0;
                        ++m_caretPosition;
                    }
                    break;
                case (char)Keys.Home:
                    m_selectionLength = shiftPressed ? m_caretPosition : 0;
                    m_caretPosition = 0;
                    break;
                case (char)Keys.End:
                    m_selectionLength = shiftPressed ? m_caretPosition - m_text.Length: 0;
                    m_caretPosition = m_text.Length;
                    break;
                case (char)Keys.Back:
                    if (m_selectionLength != 0)
                    {
                        DeleteSelection();
                    }
                    else if (m_caretPosition != 0)
                    {
                        m_text.Remove(--m_caretPosition, 1);
                    }
                    break;
                case (char)Keys.Delete:
                    if (m_selectionLength != 0)
                    {
                        DeleteSelection();
                    }
                    else if (m_caretPosition < m_text.Length)
                    {
                        m_text.Remove(m_caretPosition, 1);
                    }
                    break;
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
            if (m_inComposition)
            {
                caretOffset += m_compositionString.MeasureWidth(0, m_compositionCursorPos);
            }
            if (caretOffset < m_scrollPosition)
            {
                SetScrollPosition(Math.Max(caretOffset - InputAreaWidth / 3.0f, 0));
            }
            else if (caretOffset > m_scrollPosition + InputAreaWidth)
            {
                SetScrollPosition(Math.Max(caretOffset + InputAreaWidth / 3.0f - InputAreaWidth, 0));
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
                m_scrollPosition = nextValue;
                m_scrollSlideTrack.Play();
            }
            else
            {
                m_scrollPosition = nextValue;
            }
        }
    }
}
