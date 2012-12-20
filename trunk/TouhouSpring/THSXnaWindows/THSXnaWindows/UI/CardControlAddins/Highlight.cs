using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TouhouSpring.UI.CardControlAddins
{
    class Highlight : CardControl.Addin, Style.IBindingProvider
    {
        private Graphics.TexturedQuad m_quadHighlight;
        private Animation.Track m_borderBlinkTrack;
        private Animation.Track m_enlargeTrack;
        private bool m_lastMouseEntered;

        public Highlight(CardControl control) : base(control)
        {
            Control.Style.RegisterBinding(this);

            m_quadHighlight = new Graphics.TexturedQuad(GameApp.Service<Services.ResourceManager>().Acquire<Graphics.VirtualTexture>("Textures/CardHighlight"));
            m_quadHighlight.BlendState = new BlendState { ColorSourceBlend = Blend.SourceAlpha, ColorDestinationBlend = Blend.One };
            m_quadHighlight.ColorToModulate = Color.Lime;

            m_borderBlinkTrack = new Animation.CurveTrack(GameApp.Service<Services.ResourceManager>().Acquire<Curve>("Curve_CardFloat"));
            m_borderBlinkTrack.Elapsed += w =>
            {
                m_quadHighlight.ColorToModulate.A = (byte)(((Control.MouseTracked.MouseEntered ? 1.0f : w) / 2 + 0.5f) * 255);
            };
            m_borderBlinkTrack.Loop = true;
            m_borderBlinkTrack.Play();

            m_enlargeTrack = new Animation.CurveTrack(GameApp.Service<Services.ResourceManager>().Acquire<Curve>("Curve_CardEnlarge"));
        }

        public override void Update(float deltaTime)
        {
            if (Control.MouseTracked.MouseEntered != m_lastMouseEntered)
            {
                if (m_enlargeTrack.IsPlaying)
                {
                    m_enlargeTrack.Stop();
                }

                m_enlargeTrack.PlayFrom(0);
                if (!Control.MouseTracked.MouseEntered)
                {
                    m_enlargeTrack.Stop();
                }

                m_lastMouseEntered = Control.MouseTracked.MouseEntered;
            }

            if (m_enlargeTrack.IsPlaying)
            {
                m_enlargeTrack.Elapse(deltaTime);
            }

            m_borderBlinkTrack.Elapse(deltaTime);
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

        public bool TryGetValue(string id, out string replacement)
        {
            switch (id)
            {
                case "CardAnimator.HighlightTransform":
                    var halfWidth = Control.Region.Width * 0.5f;
                    var halfHeight = Control.Region.Height * 0.5f;
                    var scale = 1.0f + m_enlargeTrack.CurrentValue * 0.1f;
                    replacement = (MatrixHelper.Translate(-halfWidth, -halfHeight)
                                   * MatrixHelper.Scale(scale, scale)
                                   * MatrixHelper.Translate(halfWidth, halfHeight)).Serialize();
                    return true;

                default:
                    replacement = null;
                    return false;
            }
        }
    }
}
