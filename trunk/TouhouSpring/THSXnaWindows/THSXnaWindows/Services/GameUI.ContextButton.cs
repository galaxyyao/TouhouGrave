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
        private List<UI.Button> m_contextButtons = new List<UI.Button>();

        private Graphics.TexturedQuad m_buttonFace;
        private Graphics.TextRenderer.FormatOptions m_buttonTextFormatOptions;

        public void AddContextButton(string buttonText, Action<string> onClicked)
        {
            var newButton = new UI.Button
            {
                NormalFace = m_buttonFace,
                Dispatcher = InGameUIPage.Style.ChildIds["ContextButtons"].Target
            };
            newButton.ButtonText = GameApp.Service<Graphics.TextRenderer>().FormatText(buttonText, m_buttonTextFormatOptions);
            newButton.MouseButton1Up += delegate(object sender, UI.MouseEventArgs e)
            {
                if (onClicked != null) { onClicked(((UI.Button)sender).ButtonText.Text); }
            };
            var leftTop = new Point(0, m_contextButtons.Count > 0 ? m_contextButtons.Last().Region.Bottom + 10 : 0);
            newButton.Region = new Rectangle(leftTop, newButton.Region.Size);
            m_contextButtons.Add(newButton);
        }

        public void RemoveAllContextButtons()
        {
            m_contextButtons.ForEach(btn => btn.Dispatcher = null);
            m_contextButtons.Clear();
        }

		private void InitializeContextButton()
		{
			var device = GameApp.Instance.GraphicsDevice;
			var content = GameApp.Instance.Content;

			var resourceMgr = GameApp.Service<ResourceManager>();
			var buttonTexture = resourceMgr.Acquire<Graphics.VirtualTexture>("Textures/Button");
            m_buttonFace = new Graphics.TexturedQuad(buttonTexture);

            var font = new Graphics.TextRenderer.FontDescriptor("Segoe UI Light", 16);
            m_buttonTextFormatOptions = new Graphics.TextRenderer.FormatOptions(font);
		}

		private void DestroyContextButton()
		{
			GameApp.Service<ResourceManager>().Release(m_buttonFace.Texture);
		}
	}
}
