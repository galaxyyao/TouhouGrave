﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XnaColor = Microsoft.Xna.Framework.Color;

namespace TouhouSpring.UI
{
	class Button : MouseTrackedControl, IRenderable
	{
		private Graphics.TexturedQuad m_normalFace;
		private Graphics.TexturedQuad m_hoverFace;
		private Graphics.TexturedQuad m_pressedFace;

		public Graphics.TexturedQuad NormalFace
		{
			get { return m_normalFace; }
			set
			{
				if (value != null && value.Texture == null)
				{
					throw new ArgumentException("Texture can't be null.");
				}

				if (value != null)
				{
					Region = new Rectangle(0, 0, value.Texture.Width, value.Texture.Height);
				}

				m_normalFace = value;
			}
		}

		public Graphics.TexturedQuad HoverFace
		{
			get { return m_hoverFace; }
			set
			{
				if (value != null && value.Texture == null)
				{
					throw new ArgumentException("Texture can't be null.");
				}

				m_hoverFace = value;
			}
		}

		public Graphics.TexturedQuad PressedFace
		{
			get { return m_pressedFace; }
			set
			{
				if (value != null && value.Texture == null)
				{
					throw new ArgumentException("Texture can't be null.");
				}

				m_pressedFace = value;
			}
		}

		public Graphics.TextRenderer.IFormatedText ButtonText
		{
			get; set;
		}

		public XnaColor TextColor
		{
			get; set;
		}

		public Size Size
		{
			get { return Region.Size; }
		}

		public Button()
		{
			m_renderable = new Renderable(this);
			TextColor = XnaColor.White;
		}

		#region IRenderable interface

		private Renderable m_renderable;

		public void OnRender(RenderEventArgs e)
		{
			var transform = TransformToGlobal;

			var face = IsMouseButton1Pressed && PressedFace != null
					   ? PressedFace
					   : IsMouseOver && HoverFace != null
						 ? HoverFace : NormalFace;
			if (face != null)
			{
				e.RenderManager.Draw(face, Region, transform);
			}

			if (ButtonText != null)
			{
				Point position = Region.LeftTop + (Region.Size - ButtonText.Size) / 2.0f;
                var drawOptions = Graphics.TextRenderer.DrawOptions.Default;
                drawOptions.ColorScaling = TextColor.ToVector4();
                drawOptions.Offset = position;
                e.TextRenderer.DrawText(ButtonText, transform, drawOptions);
			}
		}

		#endregion
	}
}
