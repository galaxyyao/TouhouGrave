using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TouhouSpring.UI.CardControlAddins
{
	class Glow : CardControl.Addin
	{
		private Graphics.TexturedQuad m_quadGlow;

		public Color GlowColor
		{
			get; set;
		}

		public Glow(CardControl control) : base(control)
		{
			m_quadGlow = new Graphics.TexturedQuad(GameApp.Service<Services.ResourceManager>().Acquire<Graphics.VirtualTexture>("Textures/CardGlow"));
			m_quadGlow.BlendState = BlendState.AlphaBlend;
		}

		public override void Dispose()
		{
			GameApp.Service<Services.ResourceManager>().Release(m_quadGlow.Texture);
		}

		public override void RenderPostMain(Matrix transform, RenderEventArgs e)
		{
			m_quadGlow.ColorToModulate = GlowColor;
			Rectangle region = new Rectangle(Control.Region.Left - 15, Control.Region.Top - 15, Control.Region.Width + 30, Control.Region.Height + 30);
			e.RenderManager.Draw(m_quadGlow, region, transform);
		}
	}
}
