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

            m_borderBlinkTrack = new Animation.CurveTrack(GameApp.Service<Services.ResourceManager>().Acquire<Curve>("Curve_CardFloat"));
            m_borderBlinkTrack.Loop = true;
            m_borderBlinkTrack.Play();

            m_enlargeTrack = new Animation.CurveTrack(GameApp.Service<Services.ResourceManager>().Acquire<Curve>("Curve_CardEnlarge"));
        }

        public override void Update(float deltaTime)
        {
            var gameUI = GameApp.Service<Services.GameUI>();
            if (!Control.MouseTracked.MouseEntered)
            {
                m_enlargeTrack.Seek(gameUI.IsCardSelected(Control) ? m_enlargeTrack.Duration : 0);
                if (m_enlargeTrack.IsPlaying)
                {
                    m_enlargeTrack.Stop();
                }
                m_lastMouseEntered = false;
            }
            else if (Control.MouseTracked.MouseEntered != m_lastMouseEntered && gameUI.IsCardClickable(Control))
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
            var gameUI = GameApp.Service<Services.GameUI>(); ;
            bool highlightable = gameUI.ZoomedInCard != Control && gameUI.IsCardClickable(Control);

            if (highlightable
                && GameApp.Service<Services.GameUI>().ZoomedInCard != Control)
            {
                var xo = (m_quadHighlight.Texture.Width - Control.Region.Width) / 2;
                var yo = (m_quadHighlight.Texture.Height - Control.Region.Height) / 2;
                var region = new Rectangle(Control.Region.Left - xo, Control.Region.Top - yo, m_quadHighlight.Texture.Width, m_quadHighlight.Texture.Height);
                var selected = gameUI.IsCardSelected(Control);
                var color = selected ? Color.Red : Color.Lime;
                m_quadHighlight.ColorToModulate.R = color.R;
                m_quadHighlight.ColorToModulate.G = color.G;
                m_quadHighlight.ColorToModulate.B = color.B;
                m_quadHighlight.ColorToModulate.A = (byte)(selected || Control.MouseTracked.MouseEntered ? 255 : (m_borderBlinkTrack.CurrentValue / 2 + 0.5f) * 255);

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
