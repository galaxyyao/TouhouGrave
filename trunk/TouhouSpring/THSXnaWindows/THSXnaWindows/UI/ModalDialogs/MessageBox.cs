using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XnaColor = Microsoft.Xna.Framework.Color;
using TextRenderer = TouhouSpring.Graphics.TextRenderer;

namespace TouhouSpring.UI.ModalDialogs
{
    class MessageBox : TransformNode, ModalDialog.IContent, IRenderable
    {
        public const int ButtonOK       = CommonButtons.ButtonOK;
        public const int ButtonCancel   = CommonButtons.ButtonCancel;
        public const int ButtonYes      = CommonButtons.ButtonYes;
        public const int ButtonNo       = CommonButtons.ButtonNo;
        public const int NumButtons     = CommonButtons.NumButtons;

        [Flags]
        public enum ButtonFlags : uint
        {
            OK      = 1U << ButtonOK,
            Cancel  = 1U << ButtonCancel,
            Yes     = 1U << ButtonYes,
            No      = 1U << ButtonNo,
        }

        private TextRenderer.IFormattedText m_text;
        private CommonButtons m_commonButtons;
        private RenderableProxy m_renderableProxy;

        public event Action<int> ButtonClicked;

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

        public Graphics.TexturedQuad ButtonFace
        {
            get { return m_commonButtons.ButtonFace; }
            set { m_commonButtons.ButtonFace = value; LayoutButtons(); }
        }

        public TextRenderer.IFormattedText[] ButtonTexts
        {
            get { return m_commonButtons.ButtonTexts; }
            set { m_commonButtons.ButtonTexts = value; LayoutButtons(); }
        }

        public MessageBox(Graphics.TexturedQuad buttonFace, TextRenderer.IFormattedText[] buttonTexts)
        {
            m_commonButtons = new CommonButtons(buttonFace, buttonTexts);
            m_commonButtons.Dispatcher = this;
            m_commonButtons.ButtonClicked += btn =>
            {
                if (ButtonClicked != null)
                {
                    ButtonClicked(btn);
                }
            };
            m_renderableProxy = new RenderableProxy(this);
        }

        private void LayoutButtons()
        {
            var screenWidth = GameApp.Service<Services.UIManager>().ViewportWidth;
            var screenHeight = GameApp.Service<Services.UIManager>().ViewportHeight;

            const float intervalV = 20;
            float x = screenWidth / 2;
            float y = m_text != null ? (screenHeight + m_text.Size.Height + m_commonButtons.Height) / 2 + intervalV : 0;
            m_commonButtons.Transform = MatrixHelper.Translate(x, y);
        }

        void ModalDialog.IContent.OnUpdate(float deltaTime) { }
        void ModalDialog.IContent.OnPreRender() { }

        void IRenderable.OnRender(RenderEventArgs e)
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
                drawOptions.OutlineColor = XnaColor.Black.ToVector4();
                drawOptions.Offset = new Point(textLeft, textTop);
                e.TextRenderer.DrawText(m_text, transform, drawOptions);
            }
        }
    }
}
