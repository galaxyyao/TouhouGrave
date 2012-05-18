using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using XnaMatrix = Microsoft.Xna.Framework.Matrix;

namespace TouhouSpring.UI.CardControlAddins
{
	class Highlight : CardControl.Addin
	{
		private Graphics.TexturedQuad m_quadHighlight;

		public Highlight(CardControl control) : base(control)
		{
			m_quadHighlight = new Graphics.TexturedQuad(GameApp.Service<Services.ResourceManager>().Acquire<Graphics.VirtualTexture>("Textures/CardHighlight"));
			m_quadHighlight.BlendState = BlendState.Additive;
		}

		public override void Dispose()
		{
			GameApp.Service<Services.ResourceManager>().Release(m_quadHighlight.Texture);
		}

		public override void RenderMain(XnaMatrix transform, RenderEventArgs e)
		{
            bool highlightable = Control.Brightness == 1f && Control.Saturate == 1f;

            if (Control.MouseTracked.MouseEntered && highlightable)
			{
				e.RenderManager.Draw(m_quadHighlight, Control.Region, transform);
			}
		}
	}
}
