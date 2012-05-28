using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Services
{
    partial class GameUI
    {
        public enum DrawCardButton
        {
            ShowButton, HideButton
        }

        private UI.Button m_drawCardButton;
        private Graphics.TextBuffer m_drawCardButtonText;

        public void SetDrawCardButton(DrawCardButton button)
        {
            switch (button)
            {
                case DrawCardButton.ShowButton:
                    m_drawCardButton.ButtonText = m_drawCardButtonText;
                    m_drawCardButton.Dispatcher = InGameUIPage.Style.ChildIds["DrawCardButton"].Target;
                    break;
                case DrawCardButton.HideButton:
                    m_drawCardButton.Dispatcher = null;
                    break;
                default:
                    break;
            }
        }

        private void InitializeDrawCardButton()
        {
            var device = GameApp.Instance.GraphicsDevice;
            var content = GameApp.Instance.Content;

            var resourceMgr = GameApp.Service<ResourceManager>();
            m_buttonFace = resourceMgr.Acquire<Graphics.VirtualTexture>("Textures/Button");

            m_drawCardButton = new UI.Button
            {
                NormalFace = new Graphics.TexturedQuad(m_buttonFace)
            };
            m_drawCardButton.Transform = MatrixHelper.Translate(
                (device.Viewport.Width - m_drawCardButton.Region.Width) / 2,
                (device.Viewport.Height - m_drawCardButton.Region.Height) / 2 + m_drawCardButton.Region.Height * 3 / 2);
            m_drawCardButton.MouseButton1Up += (sender, e) =>
            {
                DrawCardButtonClicked();
            };

            using (var font = new System.Drawing.Font("Segoe UI Light", 16))
            {
                m_drawCardButtonText = new Graphics.TextBuffer("Draw Card", font, device);
            }
        }

        private void DrawCardButtonClicked()
        {
            m_drawCardButton.Dispatcher = null;
            var io = InteractionObject;
            TacticalPhase_OnDrawCardButton(io as Interactions.TacticalPhase);
        }
    }
}
