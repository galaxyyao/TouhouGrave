using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TextRenderer = TouhouSpring.Graphics.TextRenderer;

namespace TouhouSpring.UI.ModalDialogs
{
    class CommonButtons : TransformNode
    {
        public const int ButtonOK = 0;
        public const int ButtonCancel = ButtonOK + 1;
        public const int ButtonYes = ButtonCancel + 1;
        public const int ButtonNo = ButtonYes + 1;
        public const int NumButtons = ButtonNo + 1;

        private Graphics.TexturedQuad m_buttonFace;
        private Graphics.TexturedQuad m_buttonFaceDisabled;
        private TextRenderer.IFormattedText[] m_buttonTexts;

        private Button[] m_buttons = new Button[NumButtons];

        public event Action<int> ButtonClicked;

        public Graphics.TexturedQuad ButtonFace
        {
            get { return m_buttonFace; }
            set
            {
                if (value != m_buttonFace)
                {
                    if (value == null)
                    {
                        throw new ArgumentNullException("value");
                    }
                    m_buttonFace = value;
                    LayoutButtons();
                }
            }
        }

        public Graphics.TexturedQuad ButtonFaceDisabled
        {
            get { return m_buttonFaceDisabled; }
            set
            {
                if (value != m_buttonFaceDisabled)
                {
                    if (value == null)
                    {
                        throw new ArgumentNullException("value");
                    }
                    m_buttonFaceDisabled = value;
                    LayoutButtons();
                }
            }
        }

        public TextRenderer.IFormattedText[] ButtonTexts
        {
            get { return m_buttonTexts; }
            set
            {
                if (value != m_buttonTexts)
                {
                    if (value == null)
                    {
                        throw new ArgumentNullException("value");
                    }
                    else if (m_buttonTexts.Length != 4 || m_buttonTexts.All(txt => txt == null))
                    {
                        throw new ArgumentException("ButtonTexts array is not provided appropriately.");
                    }

                    m_buttonTexts = value;
                    CreateButtons();
                }
            }
        }

        public float Width
        {
            get; private set;
        }

        public float Height
        {
            get; private set;
        }

        public CommonButtons(Graphics.TexturedQuad buttonFace, TextRenderer.IFormattedText[] buttonTexts)
        {
            if (buttonFace == null)
            {
                throw new ArgumentNullException("buttonFace");
            }
            else if (buttonTexts == null)
            {
                throw new ArgumentNullException("buttonTexts");
            }
            else if (buttonTexts.Length != 4 || buttonTexts.All(txt => txt == null))
            {
                throw new ArgumentException("ButtonTexts array is not provided appropriately.");
            }

            m_buttonFace = buttonFace;
            m_buttonTexts = buttonTexts;

            CreateButtons();
        }

        public void SetEnabled(int button, bool enabled)
        {
            if (button < 0 || button > NumButtons)
            {
                throw new ArgumentOutOfRangeException("button");
            }
            else if (m_buttons[button] == null)
            {
                throw new InvalidOperationException("Button is not selected.");
            }

            m_buttons[button].Enabled = enabled;
        }

        private void CreateButtons()
        {
            for (int i = 0; i < NumButtons; ++i)
            {
                if (m_buttons[i] != null)
                {
                    m_buttons[i].Dispatcher = null;
                }

                if (m_buttonTexts[i] != null)
                {
                    var btn = new Button
                    {
                        ButtonText = m_buttonTexts[i],
                        Dispatcher = this
                    };
                    btn.MouseButton1Up += Button_MouseButton1Up;
                    m_buttons[i] = btn;
                }
                else
                {
                    m_buttons[i] = null;
                }
            }

            LayoutButtons();
        }

        private void LayoutButtons()
        {
            float screenWidth = GameApp.Service<Services.UIManager>().ViewportWidth;
            float screenHeight = GameApp.Service<Services.UIManager>().ViewportHeight;

            const float intervalH = 20;
            var selectedButtons = m_buttons.Where(btn => btn != null);
            selectedButtons.ForEach(btn => btn.NormalFace = m_buttonFace);
            Width = selectedButtons.Sum(btn => btn.Size.Width) + (selectedButtons.Count() - 1) * intervalH;
            Height = m_buttonFace.Texture.Height;
            float x = -Width / 2;
            float y = -Height / 2;
            foreach (var btn in selectedButtons)
            {
                btn.NormalFace = m_buttonFace;
                btn.DisabledFace = m_buttonFaceDisabled;
                btn.Transform = MatrixHelper.Translate(x, y);
                x += btn.Size.Width + intervalH;
            }
        }

        private void Button_MouseButton1Up(object sender, MouseEventArgs e)
        {
            if (ButtonClicked != null)
            {
                ButtonClicked(Array.IndexOf(m_buttons, sender));
            }
        }
    }
}
