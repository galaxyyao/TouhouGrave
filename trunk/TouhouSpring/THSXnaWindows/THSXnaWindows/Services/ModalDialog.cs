using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Services
{
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
		private Graphics.TextBuffer[] m_buttonTexts = new Graphics.TextBuffer[4];
		private System.Drawing.Font m_msgFont;

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

			Graphics.TextBuffer messageText = new Graphics.TextBuffer(message, m_msgFont, GameApp.Instance.GraphicsDevice);
			UI.ModalDialog dialog = new UI.ModalDialog(messageText);

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

			using (var font = new System.Drawing.Font("Segoe UI", 16))
			{
				m_buttonTexts[OK] = new Graphics.TextBuffer("OK", font, device);
				m_buttonTexts[Cancel] = new Graphics.TextBuffer("Cancel", font, device);
				m_buttonTexts[Yes] = new Graphics.TextBuffer("Yes", font, device);
				m_buttonTexts[No] = new Graphics.TextBuffer("No", font, device);
			}

			m_msgFont = new System.Drawing.Font("Segoe UI Light", 32);
		}

		public override void Shutdown()
		{
			m_msgFont.Dispose();
			m_buttonTexts[No].Dispose();
			m_buttonTexts[Yes].Dispose();
			m_buttonTexts[Cancel].Dispose();
			m_buttonTexts[OK].Dispose();
			GameApp.Service<ResourceManager>().Release(m_buttonFace.Texture);
		}
	}
}
