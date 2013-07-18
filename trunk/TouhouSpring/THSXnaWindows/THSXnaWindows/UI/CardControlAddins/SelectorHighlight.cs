using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TouhouSpring.UI.CardControlAddins
{
    class SelectorHighlight : CardControl.Addin
    {
        public interface IControlUI
        {
            bool IsCardSelected(CardControl cardControl);
        }

        private IControlUI m_ui;
        private Graphics.TexturedQuad m_quadHighlight;
        private Animation.Track m_borderBlinkTrack;

        public SelectorHighlight(CardControl control, IControlUI ui)
            : base(control)
        {
            m_ui = ui;

            m_quadHighlight = new Graphics.TexturedQuad(GameApp.Service<Highlight.Resources>().CardHighlight);
            m_quadHighlight.BlendState = new BlendState
            {
                ColorSourceBlend = Blend.SourceAlpha,
                ColorDestinationBlend = Blend.One,
                AlphaBlendFunction = BlendFunction.Max,
                AlphaSourceBlend = Blend.One,
                AlphaDestinationBlend = Blend.One
            };

            m_borderBlinkTrack = new Animation.CurveTrack(GameApp.Service<Highlight.Resources>().CardFloat);
            m_borderBlinkTrack.Loop = true;
            m_borderBlinkTrack.Play();
        }

        public override void Update(float deltaTime)
        {
            m_borderBlinkTrack.Elapse(deltaTime);
        }

        public override void RenderPostMain(Matrix transform, RenderEventArgs e)
        {
            if (m_ui.IsCardSelected(Control))
            {
                var xo = (m_quadHighlight.Texture.Width - Control.Region.Width) / 2;
                var yo = (m_quadHighlight.Texture.Height - Control.Region.Height) / 2;
                var region = new Rectangle(Control.Region.Left - xo, Control.Region.Top - yo, m_quadHighlight.Texture.Width, m_quadHighlight.Texture.Height);
                Color color = Color.Lime;

                m_quadHighlight.ColorToModulate.R = color.R;
                m_quadHighlight.ColorToModulate.G = color.G;
                m_quadHighlight.ColorToModulate.B = color.B;
                m_quadHighlight.ColorToModulate.A = (byte)((m_borderBlinkTrack.CurrentValue / 4 + 0.75f) * 255);

                e.RenderManager.Draw(m_quadHighlight, region, transform);
            }
        }
    }
}
