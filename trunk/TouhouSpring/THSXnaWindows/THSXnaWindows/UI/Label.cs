using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XnaColor = Microsoft.Xna.Framework.Color;

namespace TouhouSpring.UI
{
	class Label : TransformNode, IRenderable
	{
		private Renderable m_renderable;

		public Graphics.TextBuffer TextBuffer
		{
			get; set;
		}

		public bool Shadowed
		{
			get; set;
		}

		public XnaColor TextColor
		{
			get; set;
		}

		public XnaColor ShadowTextColor
		{
			get; set;
		}

		public Point ShadowOffset
		{
			get; set;
		}

		public Label()
		{
			m_renderable = new Renderable(this);

			TextColor = XnaColor.Black;
		}

		public void OnRender(RenderEventArgs e)
		{
			if (TextBuffer != null)
			{
				var transform = TransformToGlobal;
				if (Shadowed)
				{
					e.RenderManager.Draw(TextBuffer, ShadowTextColor, ShadowOffset, transform);
				}
				e.RenderManager.Draw(TextBuffer, TextColor, new Point(0, 0), transform);
			}
		}
	}
}
