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
        [Services.LifetimeDependency(typeof(Services.ResourceManager))]
        private class Resources : Services.GameService
        {
            public Graphics.VirtualTexture CardGlow
            {
                get; private set;
            }

            public override void Startup()
            {
                CardGlow = GameApp.Service<Services.ResourceManager>().Acquire<Graphics.VirtualTexture>("Textures/CardGlow");
            }

            public override void Shutdown()
            {
                GameApp.Service<Services.ResourceManager>().Release(CardGlow);
            }
        }

        private Graphics.TexturedQuad m_quadGlow;

        public Color GlowColor
        {
            get; set;
        }

        public Glow(CardControl control) : base(control)
        {
            m_quadGlow = new Graphics.TexturedQuad(GameApp.Service<Resources>().CardGlow);
            m_quadGlow.BlendState = BlendState.AlphaBlend;
        }

        public override void RenderPostMain(Matrix transform, RenderEventArgs e)
        {
            m_quadGlow.ColorToModulate = GlowColor;
            Rectangle region = new Rectangle(Control.Region.Left - 15, Control.Region.Top - 15, Control.Region.Width + 30, Control.Region.Height + 30);
            e.RenderManager.Draw(m_quadGlow, region, transform);
        }
    }
}
