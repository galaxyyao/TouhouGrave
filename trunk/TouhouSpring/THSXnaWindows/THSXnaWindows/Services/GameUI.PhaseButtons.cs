using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace TouhouSpring.Services
{
    [LifetimeDependency(typeof(Graphics.TextRenderer))]
	partial class GameUI
	{
		public enum PhaseButtonText
		{
			Next,
			Done,
			Skip,
			Draw
		}

        private List<UI.Button> m_phaseButtons = new List<UI.Button>();

        private Graphics.TexturedQuad m_buttonFace;
        private Graphics.TextRenderer.IFormattedText[] m_buttonTexts;

        public void SetSinglePhaseButton(PhaseButtonText buttonText)
        {
            RemoveAllPhaseButtons();
            AddPhaseButton(buttonText);
        }

        public void AddPhaseButton(PhaseButtonText buttonText)
        {
            var newButton = CreatePhaseButton();
            newButton.ButtonText = m_buttonTexts[(int)buttonText];
            var leftTop = new Point(0, m_phaseButtons.Count > 0 ? m_phaseButtons.Last().Region.Bottom + 10 : 0);
            newButton.Region = new Rectangle(leftTop, newButton.Region.Size);
            m_phaseButtons.Add(newButton);
        }

        public void RemoveAllPhaseButtons()
        {
            m_phaseButtons.ForEach(btn => btn.Dispatcher = null);
            m_phaseButtons.Clear();
        }

		private void InitializePhaseButtons()
		{
			var device = GameApp.Instance.GraphicsDevice;
			var content = GameApp.Instance.Content;

			var resourceMgr = GameApp.Service<ResourceManager>();
			var buttonTexture = resourceMgr.Acquire<Graphics.VirtualTexture>("Textures/Button");
            m_buttonFace = new Graphics.TexturedQuad(buttonTexture);

            m_buttonTexts = new Graphics.TextRenderer.IFormattedText[4];
            var font = new Graphics.TextRenderer.FontDescriptor("Segoe UI Light", 16);
            m_buttonTexts[(int)PhaseButtonText.Next] = GameApp.Service<Graphics.TextRenderer>().FormatText("Next", new Graphics.TextRenderer.FormatOptions(font));
            m_buttonTexts[(int)PhaseButtonText.Done] = GameApp.Service<Graphics.TextRenderer>().FormatText("Done", new Graphics.TextRenderer.FormatOptions(font));
            m_buttonTexts[(int)PhaseButtonText.Skip] = GameApp.Service<Graphics.TextRenderer>().FormatText("Skip", new Graphics.TextRenderer.FormatOptions(font));
            m_buttonTexts[(int)PhaseButtonText.Draw] = GameApp.Service<Graphics.TextRenderer>().FormatText("Draw", new Graphics.TextRenderer.FormatOptions(font));
		}

		private void DestroyPhaseButtons()
		{
			GameApp.Service<ResourceManager>().Release(m_buttonFace.Texture);
		}

        private void PhaseButtonClicked(PhaseButtonText buttonText)
        {
            UIState.OnPhaseButton(buttonText);
        }

        private UI.Button CreatePhaseButton()
        {
            var btn = new UI.Button
            {
                NormalFace = m_buttonFace,
                Dispatcher = InGameUIPage.Style.ChildIds["PhaseButton"].Target
            };
            btn.MouseButton1Up += PhaseButton_MouseButton1Up;
            return btn;
        }

        private void PhaseButton_MouseButton1Up(object sender, UI.MouseEventArgs e)
        {
            int index = m_buttonTexts.FindIndex(i => (sender as UI.Button).ButtonText == i);
            PhaseButtonClicked((PhaseButtonText)index);
        }
	}
}
