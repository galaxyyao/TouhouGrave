using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XnaColor = Microsoft.Xna.Framework.Color;
using TextRenderer = TouhouSpring.Graphics.TextRenderer;

namespace TouhouSpring.UI.ModalDialogs
{
    class MessageBox : TransformNode, IRenderable
    {
        public const int ButtonOK       = 0;
        public const int ButtonCancel   = ButtonOK + 1;
        public const int ButtonYes      = ButtonCancel + 1;
        public const int ButtonNo       = ButtonYes + 1;
        public const int NumButtons     = ButtonNo + 1;

        [Flags]
        public enum ButtonFlags : uint
        {
            OK      = 1U << ButtonOK,
            Cancel  = 1U << ButtonCancel,
            Yes     = 1U << ButtonYes,
            No      = 1U << ButtonNo,
        }

        private ButtonFlags m_buttonFlags;
        private Graphics.TexturedQuad m_buttonFace;
        private TextRenderer.IFormattedText[] m_buttonTexts;
        private TextRenderer.IFormattedText m_text;
        private Button[] m_buttons = new Button[NumButtons];

        private Renderable m_renderable;

        public event Action<int> ButtonClicked;

        public ButtonFlags Buttons
        {
            get { return m_buttonFlags; }
            set
            {
                if (value != m_buttonFlags)
                {
                    m_buttonFlags = value;
                    CreateButtons();
                }
            }
        }

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
                    else if (m_buttonTexts.Length != 4 || m_buttonTexts.Contains(null))
                    {
                        throw new ArgumentException("ButtonTexts array is not provided appropriately.");
                    }

                    m_buttonTexts = value;
                    LayoutButtons();
                }
            }
        }

        public TextRenderer.IFormattedText Text
        {
            get { return m_text; }
            set
            {
                if (value != m_text)
                {
                    m_text = value;
                    LayoutButtons();
                }
            }
        }

        public MessageBox(Graphics.TexturedQuad buttonFace, TextRenderer.IFormattedText[] buttonTexts)
        {
            m_buttonFlags = ButtonFlags.OK;

            if (buttonFace == null)
            {
                throw new ArgumentNullException("buttonFace");
            }
            else if (buttonTexts == null)
            {
                throw new ArgumentNullException("buttonTexts");
            }
            else if (buttonTexts.Length != 4 || buttonTexts.Contains(null))
            {
                throw new ArgumentException("ButtonTexts array is not provided appropriately.");
            }

            m_buttonFace = buttonFace;
            m_buttonTexts = buttonTexts;

            m_renderable = new Renderable(this);

            CreateButtons();
        }

        public void OnRender(RenderEventArgs e)
        {
            if (m_text != null)
            {
                var transform = TransformToGlobal;

                var textLeft = (int)(e.RenderManager.Device.Viewport.Width - m_text.Size.Width) / 2;
                var textTop = (int)(e.RenderManager.Device.Viewport.Height - m_text.Size.Height) / 2;

                var drawOptions = Graphics.TextRenderer.DrawOptions.Default;
                drawOptions.ColorScaling = XnaColor.Black.ToVector4();
                drawOptions.Offset = new Point(textLeft + 2, textTop + 3);
                e.TextRenderer.DrawText(m_text, transform, drawOptions);
                drawOptions.ColorScaling = XnaColor.White.ToVector4();
                drawOptions.Offset = new Point(textLeft, textTop);
                e.TextRenderer.DrawText(m_text, transform, drawOptions);
            }
        }

        private void CreateButtons()
        {
            for (int i = 0; i < NumButtons; ++i)
            {
                if (((uint)m_buttonFlags & (1U << i)) != 0)
                {
                    var btn = new Button();
                    btn.MouseButton1Up += Button_MouseButton1Up;
                    btn.Dispatcher = this;
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
            const float intervalV = 20;
            var selectedButtons = m_buttons.Where(btn => btn != null);
            float allButtonsWidth = selectedButtons.Sum(btn => btn.Size.Width) + (selectedButtons.Count() - 1) * intervalH;
            float x = (screenWidth - allButtonsWidth) / 2;
            float y = (screenHeight + (m_text != null ? m_text.Size.Height : 0)) / 2 + intervalV;
            foreach (var btn in selectedButtons)
            {
                btn.NormalFace = m_buttonFace;
                btn.ButtonText = m_buttonTexts[Array.IndexOf(m_buttons, btn)];
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
