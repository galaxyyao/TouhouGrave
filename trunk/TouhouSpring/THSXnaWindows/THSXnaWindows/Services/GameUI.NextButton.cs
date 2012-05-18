using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace TouhouSpring.Services
{
	partial class GameUI
	{
		public enum NextButton
		{
			Next,
			Done,
			Skip,
			HideButton,
		}

		private UI.Button m_nextButton;
		private Graphics.VirtualTexture m_buttonFace;
		private Graphics.TextBuffer m_nextButtonNext;
		private Graphics.TextBuffer m_nextButtonDone;
		private Graphics.TextBuffer m_nextButtonSkip;

		public void SetNextButton(NextButton button)
		{
			switch (button)
			{
				case NextButton.Next:
					m_nextButton.ButtonText = m_nextButtonNext;
					break;
				case NextButton.Done:
					m_nextButton.ButtonText = m_nextButtonDone;
					break;
				case NextButton.Skip:
					m_nextButton.ButtonText = m_nextButtonSkip;
					break;
				default:
					break;
			}
			m_nextButton.Dispatcher = button != NextButton.HideButton ? InGameUIPage.Style.ChildIds["NextButton"].Target : null;
		}

		private void InitializeNextButton()
		{
			var device = GameApp.Instance.GraphicsDevice;
			var content = GameApp.Instance.Content;

			var resourceMgr = GameApp.Service<ResourceManager>();
			m_buttonFace = resourceMgr.Acquire<Graphics.VirtualTexture>("Textures/Button");

			m_nextButton = new UI.Button
			{
				NormalFace = new Graphics.TexturedQuad(m_buttonFace)
			};
			m_nextButton.Transform = MatrixHelper.Translate(
				(device.Viewport.Width - m_nextButton.Region.Width) / 2,
				(device.Viewport.Height - m_nextButton.Region.Height) / 2);
			m_nextButton.MouseButton1Up += (sender, e) =>
			{
				NextButtonClicked();
			};

			using (var font = new System.Drawing.Font("Segoe UI Light", 16))
			{
				m_nextButtonNext = new Graphics.TextBuffer("Next", font, device);
				m_nextButtonDone = new Graphics.TextBuffer("Done", font, device);
				m_nextButtonSkip = new Graphics.TextBuffer("Skip", font, device);
			}
		}

		private void DestroyNextButton()
		{
			m_nextButtonSkip.Dispose();
			m_nextButtonDone.Dispose();
			m_nextButtonNext.Dispose();
			GameApp.Service<ResourceManager>().Release(m_buttonFace);
		}

        private void NextButtonClicked()
        {
            m_nextButton.Dispatcher = null;
            var io = InteractionObject;
            if (io is Interactions.TacticalPhase)
            {
                TacticalPhase_OnNextButton(io as Interactions.TacticalPhase);
            }
            else if (io is Interactions.SelectCards)
            {
                SelectCards_OnNextButton(io as Interactions.SelectCards);
            }
            else if (io is Interactions.BlockPhase)
            {
                BlockPhase_OnNextButton(io as Interactions.BlockPhase);
            }
            InteractionObject = null;
        }
	}
}
