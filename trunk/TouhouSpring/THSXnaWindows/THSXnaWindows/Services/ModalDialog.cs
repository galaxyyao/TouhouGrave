using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Services
{
    [LifetimeDependency(typeof(ResourceManager))]
    [LifetimeDependency(typeof(Graphics.TextRenderer))]
	class ModalDialog : GameService
	{
		[Flags]
		public enum Button
		{
			OK		= 0x01,
			Cancel	= 0x02,
			Yes		= 0x04,
			No		= 0x08,
		}

		private const int OK = 0;
		private const int Cancel = 1;
		private const int Yes = 2;
		private const int No = 3;

		private Graphics.TexturedQuad m_buttonFace;
        private Graphics.TextRenderer.IFormatedText[] m_buttonTexts = new Graphics.TextRenderer.IFormatedText[4];
		private Graphics.TextRenderer.FontDescriptor m_msgFont;

		public void Show(string message)
		{
			Show(message, Button.OK, null);
		}

		public void Show(string message, Action action)
		{
			Show(message, Button.OK, action != null ? (btn) => action() : (Action<Button>)null);
		}

		public void Show(string message, Button button, Action<Button> action)
		{
			if (message == null)
			{
				throw new ArgumentNullException("message");
			}

			var dialog = new UI.ModalDialog(GameApp.Service<Graphics.TextRenderer>().FormatText(message, new Graphics.TextRenderer.FormatOptions(m_msgFont)));

			if ((button & Button.OK) != 0)
			{
				var btn = new UI.Button
				{
					NormalFace = m_buttonFace,
					ButtonText = m_buttonTexts[OK]
				};
				if (action != null)
				{
					btn.MouseButton1Up += (s, e) => action(Button.OK);
				}
				dialog.AddButton(btn);
			}
			if ((button & Button.Yes) != 0)
			{
				var btn = new UI.Button
				{
					NormalFace = m_buttonFace,
					ButtonText = m_buttonTexts[Yes]
				};
				if (action != null)
				{
					btn.MouseButton1Up += (s, e) => action(Button.Yes);
				}
				dialog.AddButton(btn);
			}
			if ((button & Button.No) != 0)
			{
				var btn = new UI.Button
				{
					NormalFace = m_buttonFace,
					ButtonText = m_buttonTexts[No]
				};
				if (action != null)
				{
					btn.MouseButton1Up += (s, e) => action(Button.No);
				}
				dialog.AddButton(btn);
			}
			if ((button & Button.Cancel) != 0)
			{
				var btn = new UI.Button
				{
					NormalFace = m_buttonFace,
					ButtonText = m_buttonTexts[Cancel]
				};
				if (action != null)
				{
					btn.MouseButton1Up += (s, e) => action(Button.Cancel);
				}
				dialog.AddButton(btn);
			}

			dialog.Begin(GameApp.Service<UIManager>().Root.Listeners.Last(l => l is UI.EventDispatcher) as UI.EventDispatcher);
		}

		public override void Startup()
		{
			var device = GameApp.Instance.GraphicsDevice;
			var resourceMgr = GameApp.Service<ResourceManager>();

			m_buttonFace = new Graphics.TexturedQuad(resourceMgr.Acquire<Graphics.VirtualTexture>("Textures/Button"));

            var font = new Graphics.TextRenderer.FontDescriptor("Segoe UI", 16);
            m_buttonTexts[OK] = GameApp.Service<Graphics.TextRenderer>().FormatText("OK", new Graphics.TextRenderer.FormatOptions(font));
            m_buttonTexts[Cancel] = GameApp.Service<Graphics.TextRenderer>().FormatText("Cancel", new Graphics.TextRenderer.FormatOptions(font));
            m_buttonTexts[Yes] = GameApp.Service<Graphics.TextRenderer>().FormatText("Yes", new Graphics.TextRenderer.FormatOptions(font));
            m_buttonTexts[No] = GameApp.Service<Graphics.TextRenderer>().FormatText("No", new Graphics.TextRenderer.FormatOptions(font));

			m_msgFont = new Graphics.TextRenderer.FontDescriptor("Segoe UI Light", 32);
		}

		public override void Shutdown()
		{
			GameApp.Service<ResourceManager>().Release(m_buttonFace.Texture);
		}
	}
}
