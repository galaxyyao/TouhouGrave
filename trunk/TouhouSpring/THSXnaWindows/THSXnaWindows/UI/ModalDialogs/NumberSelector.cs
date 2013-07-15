using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XnaColor = Microsoft.Xna.Framework.Color;
using TextRenderer = TouhouSpring.Graphics.TextRenderer;

namespace TouhouSpring.UI.ModalDialogs
{
    class NumberSelector : TransformNode, ModalDialog.IContent, IRenderable
    {
        public const int ButtonOK       = 0;
        public const int ButtonCancel   = ButtonOK + 1;
        public const int NumButtons     = ButtonCancel + 1;

        private TextRenderer.IFormattedText m_text;
        private TextRenderer.IFormattedText[] m_digits = new TextRenderer.IFormattedText[10];
        private TextRenderer.IFormattedText[] m_signs = new TextRenderer.IFormattedText[2];
        private Graphics.TexturedQuad m_upButtonFace;
        private Graphics.TexturedQuad m_downButtonFace;

        private TextRenderer.DrawOptions m_textDrawOptions = TextRenderer.DrawOptions.Default;
        private Button[] m_upButtons;
        private Button[] m_downButtons;
        private TextRenderer.DrawOptions[] m_digitsDrawOptions; // including minus sign, deciding the position of each digit

        private int m_currentValue = 0;
        private int m_sign = 0;
        private CommonButtons m_commonButtons;
        private RenderableProxy m_renderableProxy;

        public event Action<int, int> ButtonClicked;

        public TextRenderer.IFormattedText Text
        {
            get { return m_text; }
            set
            {
                if (value != m_text)
                {
                    m_text = value;
                    LayoutGizmos();
                }
            }
        }

        public TextRenderer.IFormattedText[] Digits
        {
            get { return m_digits; }
            set
            {
                if (value != m_digits)
                {
                    if (value == null)
                    {
                        throw new ArgumentNullException("value");
                    }
                    else if (value.Length != 10 || value.Contains(null))
                    {
                        throw new ArgumentException("Digits array is not provided appropriately.");
                    }

                    m_digits = value;
                    LayoutGizmos();
                }
            }
        }

        public TextRenderer.IFormattedText[] Signs
        {
            get { return m_signs; }
            set
            {
                if (value != m_signs)
                {
                    if (value == null)
                    {
                        throw new ArgumentNullException("value");
                    }
                    else if (m_signs.Length != 2 || m_signs.Contains(null))
                    {
                        throw new ArgumentException("Signs array is not provided appropriately.");
                    }

                    m_signs = value;
                    LayoutGizmos();
                }
            }
        }

        public Graphics.TexturedQuad UpButtonFace
        {
            get { return m_upButtonFace; }
            set
            {
                if (value != m_upButtonFace)
                {
                    m_upButtonFace = value;
                    UpdateUpButtons();
                    LayoutGizmos();
                }
            }
        }

        public Graphics.TexturedQuad DownButtonFace
        {
            get { return m_downButtonFace; }
            set
            {
                if (value != m_downButtonFace)
                {
                    m_downButtonFace = value;
                    UpdateDownButtons();
                    LayoutGizmos();
                }
            }
        }

        public Graphics.TexturedQuad OkCancelButtonFace
        {
            get { return m_commonButtons.ButtonFace; }
            set
            {
                if (value != m_commonButtons.ButtonFace)
                {
                    m_commonButtons.ButtonFace = value;
                    LayoutGizmos();
                }
            }
        }

        public Graphics.TexturedQuad OkCancelButtonFaceDisabled
        {
            get { return m_commonButtons.ButtonFaceDisabled; }
            set { m_commonButtons.ButtonFaceDisabled = value; }
        }

        public TextRenderer.IFormattedText[] OkCancelTexts
        {
            get
            {
                return new TextRenderer.IFormattedText[]
                {
                    m_commonButtons.ButtonTexts[CommonButtons.ButtonOK],
                    m_commonButtons.ButtonTexts[CommonButtons.ButtonCancel]
                };
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                else if (value.Length != NumButtons || value.Contains(null))
                {
                    throw new ArgumentException("OkCancelTexts array is not provided appropriately.");
                }
                if (m_commonButtons.ButtonTexts[CommonButtons.ButtonOK] != value[ButtonOK]
                    || m_commonButtons.ButtonTexts[CommonButtons.ButtonCancel] != value[ButtonCancel])
                {
                    var buttonTexts = new TextRenderer.IFormattedText[CommonButtons.NumButtons];
                    buttonTexts[CommonButtons.ButtonOK] = value[ButtonOK];
                    buttonTexts[CommonButtons.ButtonCancel] = value[ButtonCancel];
                    m_commonButtons.ButtonTexts = buttonTexts;
                }
            }
        }

        public int MinValue
        {
            get; private set;
        }

        public int MaxValue
        {
            get; private set;
        }

        public int CurrentValue
        {
            get { return m_currentValue; }
            set
            {
                if (value != m_currentValue)
                {
                    m_currentValue = value;
                    OnValueChanged();
                }
            }
        }

        public NumberSelector(TextRenderer.IFormattedText[] digits, TextRenderer.IFormattedText[] signs, Graphics.TexturedQuad upButtonFace, Graphics.TexturedQuad downButtonFace,
            Graphics.TexturedQuad okCancelButtonFace, TextRenderer.IFormattedText[] okCancelTexts)
        {
            if (digits == null)
            {
                throw new ArgumentNullException("digits");
            }
            else if (digits.Length != 10 || digits.Contains(null))
            {
                throw new ArgumentException("Digits array is not provided appropriately.");
            }
            else if (signs == null)
            {
                throw new ArgumentNullException("signs");
            }
            else if (signs.Length != 2 || signs.Contains(null))
            {
                throw new ArgumentException("Signs array is not provided appropriately.");
            }
            else if (upButtonFace == null)
            {
                throw new ArgumentNullException("upButtonFace");
            }
            else if (downButtonFace == null)
            {
                throw new ArgumentNullException("downButtonFace");
            }
            else if (okCancelTexts == null)
            {
                throw new ArgumentNullException("okCancelTexts");
            }
            else if (okCancelTexts.Length != NumButtons || okCancelTexts.Contains(null))
            {
                throw new ArgumentException("OkCancelTexts array is not provided appropriately.");
            }

            m_digits = digits;
            m_signs = signs;
            m_upButtonFace = upButtonFace;
            m_downButtonFace = downButtonFace;

            var buttonTexts = new TextRenderer.IFormattedText[CommonButtons.NumButtons];
            buttonTexts[CommonButtons.ButtonOK] = okCancelTexts[ButtonOK];
            buttonTexts[CommonButtons.ButtonCancel] = okCancelTexts[ButtonCancel];
            m_commonButtons = new CommonButtons(okCancelButtonFace, buttonTexts);
            m_commonButtons.Dispatcher = this;
            m_commonButtons.ButtonClicked += btn =>
            {
                if (ButtonClicked != null)
                {
                    ButtonClicked(btn, CurrentValue);
                }
            };
            m_renderableProxy = new RenderableProxy(this);

            SetRange(0, 1);
        }

        public void SetRange(int min, int max)
        {
            MinValue = min;
            MaxValue = max;
            m_currentValue = min;
            m_sign = m_currentValue >= 0 ? 0 : 1;

            CreateGizmos();
            UpdateUpButtons();
            UpdateDownButtons();
            LayoutGizmos();
        }

        private void CreateGizmos()
        {
            if (m_upButtons != null)
            {
                m_upButtons.ForEach(btn => btn.Dispatcher = null);
                m_downButtons.ForEach(btn => btn.Dispatcher = null);
            }

            int numDigits1 = Math.Abs(MinValue).ToString().Length;
            int numDigits2 = Math.Abs(MaxValue).ToString().Length;
            int numDigits = Math.Max(numDigits1, numDigits2);
            bool hasMinusSign = MinValue < 0 || MaxValue < 0;

            int gizmoArrayLength = numDigits + (hasMinusSign ? 1 : 0);
            m_digitsDrawOptions = new TextRenderer.DrawOptions[gizmoArrayLength];
            m_upButtons = new Button[gizmoArrayLength];
            m_downButtons = new Button[gizmoArrayLength];

            for (int i = 0; i < gizmoArrayLength; ++i)
            {
                m_digitsDrawOptions[i] = Graphics.TextRenderer.DrawOptions.Default;

                if (MinValue == MaxValue)
                {
                    continue;
                }

                if (i == numDigits)
                {
                    bool signCanBeToggled = hasMinusSign && MaxValue > 0;
                    if (!signCanBeToggled)
                    {
                        continue;
                    }
                }

                var btnUp = new Button { Dispatcher = this };
                var btnDown = new Button { Dispatcher = this };
                btnUp.MouseButton1Up += UpDownButton_Clicked;
                btnDown.MouseButton1Up += UpDownButton_Clicked;

                m_upButtons[i] = btnUp;
                m_downButtons[i] = btnDown;
            }
        }

        private void LayoutGizmos()
        {
            const float intervalH = 10;
            const float intervalV = 5;

            // get the widest digit character
            float maxWidth = m_digits.Max(digit => digit.Size.Width);
            maxWidth = Math.Max(maxWidth, m_signs.Max(sign => sign.Size.Width));
            maxWidth = Math.Max(maxWidth, m_upButtonFace.Texture.Width);
            maxWidth = Math.Max(maxWidth, m_downButtonFace.Texture.Width);

            float maxHeight = m_digits.Max(digit => digit.Size.Height);
            maxHeight = Math.Max(maxHeight, m_signs.Max(sign => sign.Size.Height));

            float totalWidth = m_digitsDrawOptions.Length * (maxWidth + intervalH) - intervalH;
            float screenWidth = GameApp.Service<Services.UIManager>().ViewportWidth;
            float marginRight = (screenWidth + totalWidth) / 2;

            float screenHeight = GameApp.Service<Services.UIManager>().ViewportHeight;
            float marginTop = (screenHeight - maxHeight) / 2;
            float marginBottom = (screenHeight + maxHeight) / 2;

            for (int i = 0; i < m_digitsDrawOptions.Length; ++i)
            {
                var center = marginRight - maxWidth * 0.5f - i * (maxWidth + intervalH);
                m_digitsDrawOptions[i].Offset.X = center;
                m_digitsDrawOptions[i].Offset.Y = screenHeight / 2;

                if (m_upButtons[i] != null)
                {
                    m_upButtons[i].Transform = MatrixHelper.Translate(center - m_upButtonFace.Texture.Width / 2, marginTop - intervalV - m_upButtonFace.Texture.Height);
                    m_downButtons[i].Transform = MatrixHelper.Translate(center - m_downButtonFace.Texture.Width / 2, marginBottom + intervalV);
                }
            }

            const float LargeIntervalV = 30;
            
            if (m_text != null)
            {
                m_textDrawOptions.Offset.X = (screenWidth - m_text.Size.Width) / 2;
                m_textDrawOptions.Offset.Y = marginTop - intervalV - m_upButtonFace.Texture.Height - LargeIntervalV - m_text.Size.Height;
            }

            m_commonButtons.Transform = MatrixHelper.Translate(screenWidth / 2, marginBottom + intervalV + m_downButtonFace.Texture.Height + LargeIntervalV + m_commonButtons.Height / 2);
        }

        private void UpdateUpButtons()
        {
            foreach (var btn in m_upButtons)
            {
                if (btn != null)
                {
                    btn.NormalFace = UpButtonFace;
                }
            }
        }

        private void UpdateDownButtons()
        {
            foreach (var btn in m_downButtons)
            {
                if (btn != null)
                {
                    btn.NormalFace = DownButtonFace;
                }
            }
        }

        private void UpDownButton_Clicked(object sender, MouseEventArgs e)
        {
            var index = Array.IndexOf(m_upButtons, sender);
            bool isUp = index != -1;
            index = isUp ? index : Array.IndexOf(m_downButtons, sender);

            int mod = (int)Math.Pow(10, Math.Abs(index));
            bool isSign = mod > Math.Max(Math.Abs(MaxValue), Math.Abs(MinValue));

            if (isSign)
            {
                m_currentValue *= -1;
                m_sign = 1 - m_sign;
            }
            else
            {
                m_currentValue = Math.Abs(m_currentValue);
                int digit = m_currentValue % (mod * 10) / mod;
                if (isUp)
                {
                    if (digit == 9)
                    {
                        m_currentValue -= mod * 9;
                    }
                    else
                    {
                        m_currentValue += mod;
                    }
                }
                else
                {
                    if (digit == 0)
                    {
                        m_currentValue += mod * 9;
                    }
                    else
                    {
                        m_currentValue -= mod;
                    }
                }
                m_currentValue *= m_sign == 0 ? 1 : -1;
            }

            OnValueChanged();
        }

        private void OnValueChanged()
        {
            m_commonButtons.SetEnabled(CommonButtons.ButtonOK, m_currentValue >= MinValue && m_currentValue <= MaxValue);
        }

        void ModalDialog.IContent.OnUpdate(float deltaTime) { }
        void ModalDialog.IContent.OnPreRender() { }

        void IRenderable.OnRender(RenderEventArgs e)
        {
            var transform = TransformToGlobal;

            if (m_text != null)
            {
                var textLeft = (int)(e.RenderManager.Device.Viewport.Width - m_text.Size.Width) / 2;
                var textTop = (int)(e.RenderManager.Device.Viewport.Height - m_text.Size.Height) / 2;

                var drawOptions = m_textDrawOptions;
                drawOptions.ColorScaling = XnaColor.Black.ToVector4();
                drawOptions.Offset.X += 2;
                drawOptions.Offset.Y += 3;
                e.TextRenderer.DrawText(m_text, transform, drawOptions);
                drawOptions.ColorScaling = XnaColor.White.ToVector4();
                drawOptions.Offset.X -= 2;
                drawOptions.Offset.Y -= 3;
                e.TextRenderer.DrawText(m_text, transform, drawOptions);
            }

            int mod = 1;
            for (int i = 0; i < m_digitsDrawOptions.Length; ++i)
            {
                int digit = Math.Abs(m_currentValue) % (mod * 10) / mod;

                var drawOptions = m_digitsDrawOptions[i];
                var digitChar = mod > Math.Max(Math.Abs(MaxValue), Math.Abs(MinValue)) ? m_signs[m_sign] : m_digits[digit];
                drawOptions.Offset.X -= digitChar.Size.Width / 2 - 2;
                drawOptions.Offset.Y -= digitChar.Size.Height / 2 - 2;
                drawOptions.ColorScaling = XnaColor.Black.ToVector4();
                e.TextRenderer.DrawText(digitChar, transform, drawOptions);
                drawOptions.ColorScaling = XnaColor.White.ToVector4();
                drawOptions.Offset.X -= 2;
                drawOptions.Offset.Y -= 2;
                e.TextRenderer.DrawText(digitChar, transform, drawOptions);

                mod *= 10;
            }
        }
    }
}
