using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace TouhouSpring.Services
{
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
        private Graphics.TextBuffer[] m_buttonTexts;

        public void SetSinglePhaseButton(PhaseButtonText buttonText)
        {
            RemoveAllPhaseButtons();
            AddPhaseButton(buttonText);
        }

        public void AddPhaseButton(PhaseButtonText buttonText)
        {
            var newButton = CreatePhaseButton();
            newButton.ButtonText = m_buttonTexts[(int)buttonText];
            var leftTop = new Point(0, m_phaseButtons.Count > 0 ? m_phaseButtons[m_phaseButtons.Count - 1].Region.Bottom + 10 : 0);
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

            m_buttonTexts = new Graphics.TextBuffer[4];
			using (var font = new System.Drawing.Font("Segoe UI Light", 16))
			{
                m_buttonTexts[(int)PhaseButtonText.Next] = new Graphics.TextBuffer("Next", font, device);
                m_buttonTexts[(int)PhaseButtonText.Done] = new Graphics.TextBuffer("Done", font, device);
                m_buttonTexts[(int)PhaseButtonText.Skip] = new Graphics.TextBuffer("Skip", font, device);
                m_buttonTexts[(int)PhaseButtonText.Draw] = new Graphics.TextBuffer("Draw", font, device);
			}
		}

		private void DestroyPhaseButtons()
		{
            m_buttonTexts.ForEach(i => i.Dispose());
			GameApp.Service<ResourceManager>().Release(m_buttonFace.Texture);
		}

        private void PhaseButtonClicked(PhaseButtonText buttonText)
        {
            bool succeeded = true;

            var io = InteractionObject;
            if (io is Interactions.TacticalPhase)
            {
                succeeded = TacticalPhase_OnPhaseButton(io as Interactions.TacticalPhase, buttonText);
            }
            else if (io is Interactions.SelectCards)
            {
                succeeded = SelectCards_OnPhaseButton(io as Interactions.SelectCards, buttonText);
            }
            else if (io is Interactions.BlockPhase)
            {
                succeeded = BlockPhase_OnPhaseButton(io as Interactions.BlockPhase, buttonText);
            }

            if (succeeded)
            {
                RemoveAllPhaseButtons();
                InteractionObject = null;
            }
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
