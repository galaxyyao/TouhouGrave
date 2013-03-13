using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TouhouSpring.UI
{
    class Panel : MouseTrackedControl, IRenderable
    {
        private RenderableProxy m_renderableProxy;

        public Color Color
        {
            get; set;
        }

        public Panel()
        {
            m_renderableProxy = new RenderableProxy(this);

            Color = Color.Black;
        }

        public void OnRender(RenderEventArgs e)
        {
            var transform = TransformToGlobal;

            Graphics.TexturedQuad quadOverlay = new Graphics.TexturedQuad();
            quadOverlay.ColorToModulate = Color;
            e.RenderManager.Draw(quadOverlay, Region, transform);
        }



    }
}
