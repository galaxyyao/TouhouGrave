using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TouhouSpring.Graphics
{
	partial class RenderManager
	{
        private TexturedQuad m_quad = new TexturedQuad
        {
            BlendState = BlendState.NonPremultiplied
        };

		public void Draw(TextBuffer textBuffer, Color color, Point position, Matrix transform)
		{
			if (textBuffer == null)
			{
				throw new ArgumentNullException("textBuffer");
			}

			m_quad.Texture = textBuffer.Texture;
			m_quad.UVBounds = new Rectangle(0, 0, textBuffer.Texture.Width, textBuffer.Texture.Height);
			m_quad.ColorToModulate = color;
			Draw(m_quad, new Point((float)Math.Round(position.X), (float)Math.Round(position.Y)), transform);
		}
	}
}
