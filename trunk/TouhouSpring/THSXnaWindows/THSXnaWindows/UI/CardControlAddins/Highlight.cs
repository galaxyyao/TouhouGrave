using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TouhouSpring.UI.CardControlAddins
{
	class Highlight : CardControl.Addin
	{
		private Graphics.TexturedQuad m_quadHighlight;
        private Animation.Track m_animationTrack;

		public Highlight(CardControl control) : base(control)
		{
			m_quadHighlight = new Graphics.TexturedQuad(GameApp.Service<Services.ResourceManager>().Acquire<Graphics.VirtualTexture>("Textures/CardHighlight"));
            m_quadHighlight.BlendState = new BlendState { ColorSourceBlend = Blend.SourceAlpha, ColorDestinationBlend = Blend.One };
            m_quadHighlight.ColorToModulate = Color.Lime;

            m_animationTrack = new Animation.CurveTrack(GameApp.Service<Services.ResourceManager>().Acquire<Curve>("Curve_CardFloat"));
            m_animationTrack.Elapsed += w =>
            {
                m_quadHighlight.ColorToModulate.A = (byte)(((Control.MouseTracked.MouseEntered ? 1.0f : w) / 2 + 0.5f) * 255);
            };
            m_animationTrack.Loop = true;
            m_animationTrack.Play();
		}

        public override void Update(float deltaTime)
        {
            m_animationTrack.Elapse(deltaTime);
        }

		public override void Dispose()
		{
			GameApp.Service<Services.ResourceManager>().Release(m_quadHighlight.Texture);
		}

        public override void RenderPostMain(Matrix transform, RenderEventArgs e)
		{
            bool highlightable = Control.Brightness == 1f && Control.Saturate == 1f;

            if (highlightable
                && GameApp.Service<Services.GameUI>().ZoomedInCard != Control)
			{
                var xo = (m_quadHighlight.Texture.Width - Control.Region.Width) / 2;
                var yo = (m_quadHighlight.Texture.Height - Control.Region.Height) / 2;
                var region = new Rectangle(Control.Region.Left - xo, Control.Region.Top - yo, m_quadHighlight.Texture.Width, m_quadHighlight.Texture.Height);
                
				e.RenderManager.Draw(m_quadHighlight, region, transform);
			}
		}
	}
}
