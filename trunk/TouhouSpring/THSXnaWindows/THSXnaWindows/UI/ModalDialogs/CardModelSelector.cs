using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XnaColor = Microsoft.Xna.Framework.Color;
using TextRenderer = TouhouSpring.Graphics.TextRenderer;

namespace TouhouSpring.UI.ModalDialogs
{
    partial class CardModelSelector : TransformNode, ModalDialog.IContent, IRenderable
    {
        public const int ButtonCancel = 0;
        public const int NumButtons = ButtonCancel + 1;

        private const int TextLineFromMiddle = -150;
        private const int LeftRightButtonLineFromMiddle = 120;
        private const int LeftRightButtonInterval = 180;
        private const int CancelButtonLineFromMiddle = 200;
        private const float CardWidth = 150;

        private TextRenderer.IFormattedText m_text;
        private Button m_leftButton;
        private Button m_rightButton;

        private CommonButtons m_commonButtons;
        private RenderableProxy m_renderableProxy;
        private TransformNode m_cardContainer;

        public event Action<int> CancelClicked;

        public TextRenderer.IFormattedText Text
        {
            get { return m_text; }
            set
            {
                if (value != m_text)
                {
                    m_text = value;
                    Layout();
                }
            }
        }

        public int Center { get; set; }

        public CardModelSelector(Graphics.TexturedQuad cancelButtonFace, TextRenderer.IFormattedText cancelButtonText,
            Graphics.TexturedQuad leftButtonFace, TextRenderer.IFormattedText leftButtonText,
            Graphics.TexturedQuad rightButtonFace, TextRenderer.IFormattedText rightButtonText)
        {
            if (cancelButtonFace == null)
            {
                throw new ArgumentNullException("cancelButtonFace");
            }
            else if (cancelButtonText == null)
            {
                throw new ArgumentNullException("cancelButtonText");
            }

            var buttonTexts = new TextRenderer.IFormattedText[CommonButtons.NumButtons];
            buttonTexts[CommonButtons.ButtonCancel] = cancelButtonText;
            m_commonButtons = new CommonButtons(cancelButtonFace, buttonTexts);
            m_commonButtons.Dispatcher = this;
            m_commonButtons.ButtonClicked += btn =>
            {
                if (CancelClicked != null)
                {
                    CancelClicked(btn);
                }
            };

            m_leftButton = new Button
            {
                ButtonText = leftButtonText,
                NormalFace = leftButtonFace,
                Dispatcher = this
            };
            m_leftButton.MouseButton1Down += (sender, e) =>
            {
                --Center;
            };

            m_rightButton = new Button
            {
                ButtonText = rightButtonText,
                NormalFace = rightButtonFace,
                Dispatcher = this
            };
            m_rightButton.MouseButton1Down += (sender, e) =>
            {
                ++Center;
            };

            m_renderableProxy = new RenderableProxy(this);
            m_cardContainer = new TransformNode
            {
                Dispatcher = this
            };
            Layout();
        }

        private void Layout()
        {
            var screenWidth = GameApp.Service<Services.UIManager>().ViewportWidth;
            var screenHeight = GameApp.Service<Services.UIManager>().ViewportHeight;

            var x = screenWidth / 2;
            var y = screenHeight / 2;

            m_cardContainer.Transform = MatrixHelper.Scale(CardWidth, -CardWidth) * MatrixHelper.Translate(x, y);

            var leftRightButtonLineWidth = m_leftButton.Size.Width + m_rightButton.Size.Width + LeftRightButtonInterval;
            m_leftButton.Transform = MatrixHelper.Translate(x - leftRightButtonLineWidth / 2, y + LeftRightButtonLineFromMiddle);
            m_rightButton.Transform = MatrixHelper.Translate(x + leftRightButtonLineWidth / 2 - m_rightButton.Size.Width, y + LeftRightButtonLineFromMiddle);

            m_commonButtons.Transform = MatrixHelper.Translate(x, y + CancelButtonLineFromMiddle);
        }

        void ModalDialog.IContent.OnUpdate(float deltaTime)
        {
            foreach (var cc in m_cardControls)
            {
                cc.Update(deltaTime);
            }
        }

        void ModalDialog.IContent.OnPreRender()
        {
        }

        void IRenderable.OnRender(RenderEventArgs e)
        {
            if (m_text != null)
            {
                var transform = TransformToGlobal;

                var textLeft = (int)(e.RenderManager.Device.Viewport.Width - m_text.Size.Width) / 2;
                var textTop = (int)(e.RenderManager.Device.Viewport.Height - m_text.Size.Height) / 2 + TextLineFromMiddle;

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
