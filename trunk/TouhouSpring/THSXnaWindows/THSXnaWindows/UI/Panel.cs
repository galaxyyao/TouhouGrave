using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TouhouSpring.UI
{
    class Panel : MouseTrackedControl, IRenderable
    {
        private Renderable m_renderable;
        public Panel()
		{
			m_renderable = new Renderable(this);
		}

        public void OnRender(RenderEventArgs e)
        {
            var transform = TransformToGlobal;

            Graphics.TexturedQuad quadOverlay = new Graphics.TexturedQuad();
            quadOverlay.ColorScale = new Vector4(1, 1, 1, 0.75f);
            e.RenderManager.Draw(quadOverlay, Region, transform);
        }



    }
}
