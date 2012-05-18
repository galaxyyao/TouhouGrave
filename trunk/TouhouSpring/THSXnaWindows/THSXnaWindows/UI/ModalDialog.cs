using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TouhouSpring.UI
{
	class ModalDialog : MouseTrackedControl, IRenderable
	{
		private List<Button> m_buttons = new List<Button>();
		private MouseEventRelay m_eventRelay;
		private Renderable m_renderable;

		public bool HasBegun
		{
			get; private set;
		}

		public override EventDispatcher Dispatcher
		{
			get { return base.Dispatcher; }
			set { throw new InvalidOperationException("Dispatcher can't be set directly."); }
		}

		public new Rectangle Region
		{
			get { return base.Region; }
		}

		public Graphics.TextBuffer Text
		{
			get; private set;
		}

		public ModalDialog(Graphics.TextBuffer text)
		{
			if (text == null)
			{
				throw new ArgumentNullException("text");
			}

			Text = text;
			m_eventRelay = new MouseEventRelay(this);
			m_eventRelay.SetHandledAfterRelay = true;
			m_renderable = new Renderable(this);
		}

		public void AddButton(Button button)
		{
			if (button == null)
			{
				throw new ArgumentNullException("button");
			}
			else if (button.Dispatcher != null)
			{
				throw new ArgumentNullException("Button is parented by another dispatcher.");
			}
			else if (m_buttons.Contains(button))
			{
				throw new ArgumentException("Button has already been added.");
			}
			else if (HasBegun)
			{
				throw new InvalidOperationException("Can't add buttons after the modal dialog has begun.");
			}

			m_buttons.Add(button);
			Listeners.Add(button);
			button.MouseButton1Up += (sender, e) => OnButtonClicked(sender, e);
		}

		public void Begin(EventDispatcher parent)
		{
			// reset size
			float screenWidth = GameApp.Service<Services.UIManager>().ViewportWidth;
			float screenHeight = GameApp.Service<Services.UIManager>().ViewportHeight;
			base.Region = new Rectangle(0, 0, screenWidth, screenHeight);

			var parentTransform = parent is ITransformNode ? (parent as ITransformNode).TransformToGlobal : Matrix.Identity;
			var toScreenSpace = Matrix.Identity;
			toScreenSpace.M11 = 2 / screenWidth;
			toScreenSpace.M22 = -2 / screenHeight;
			toScreenSpace.M41 = -1;
			toScreenSpace.M42 = 1;
			Transform = toScreenSpace * parentTransform.Invert();

			base.Dispatcher = parent;
			Dispatcher.Listeners.Add(m_eventRelay);

			// arrange the buttons
			const float intervalH = 20;
			const float intervalV = 20;
			float allButtonsWidth = m_buttons.Sum(btn => btn.Size.Width) + (m_buttons.Count - 1) * intervalH;
			float x = (screenWidth - allButtonsWidth) / 2;
			float y = (screenHeight + Text.TextSize.Height) / 2 + intervalV;
			foreach (var btn in m_buttons)
			{
				btn.Transform = MatrixHelper.Translate(x, y);
				x += btn.Size.Width + intervalH;
			}

			HasBegun = true;
		}

		public void OnRender(RenderEventArgs e)
		{
			float screenWidth = e.RenderManager.Device.Viewport.Width;
			float screenHeight = e.RenderManager.Device.Viewport.Height;
			var transform = TransformToGlobal;

			// draw a black overlay
			Graphics.TexturedQuad quadOverlay = new Graphics.TexturedQuad();
			quadOverlay.ColorScale = new Vector4(0, 0, 0, 0.75f);
			e.RenderManager.Draw(quadOverlay, new Point(-0.5f, -0.5f), transform);

			float textLeft = (screenWidth - Text.TextSize.Width) / 2;
			float textTop = (screenHeight - Text.TextSize.Height) / 2;

			e.RenderManager.Draw(Text, Color.Black, new Point(textLeft + 2, textTop + 3), transform);
			e.RenderManager.Draw(Text, Color.White, new Point(textLeft, textTop), transform);
		}

		private void OnButtonClicked(object sender, MouseEventArgs e)
		{
			Dispatcher.Listeners.Remove(m_eventRelay);
			Dispatcher.Listeners.Remove(this);
		}
	}
}
