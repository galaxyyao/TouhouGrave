using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TouhouSpring.Services.UIStates;

namespace TouhouSpring.UI.CardControlAddins
{
    class Highlight : CardControl.Addin, Style.IBindingProvider
    {
        [Services.LifetimeDependency(typeof(Services.ResourceManager))]
        public class Resources : Services.GameService
        {
            public Graphics.VirtualTexture CardHighlight
            {
                get; private set;
            }

            public Curve CardFloat
            {
                get; private set;
            }

            public Curve CardEnlarge
            {
                get; private set;
            }

            public override void Startup()
            {
                CardHighlight = GameApp.Service<Services.ResourceManager>().Acquire<Graphics.VirtualTexture>("Textures/CardHighlight");
                CardFloat = GameApp.Service<Services.ResourceManager>().Acquire<Curve>("Curves/CardFloat");
                CardEnlarge = GameApp.Service<Services.ResourceManager>().Acquire<Curve>("Curves/CardEnlarge");
            }

            public override void Shutdown()
            {
                GameApp.Service<Services.ResourceManager>().Release(CardEnlarge);
                GameApp.Service<Services.ResourceManager>().Release(CardFloat);
                GameApp.Service<Services.ResourceManager>().Release(CardHighlight);
            }
        }

        private Graphics.TexturedQuad m_quadHighlight;
        private Animation.Track m_borderBlinkTrack;
        private Animation.Track m_enlargeTrack;
        private bool m_lastMouseEntered;

        public Highlight(CardControl control) : base(control)
        {
            Control.Style.RegisterBinding(this);

            m_quadHighlight = new Graphics.TexturedQuad(GameApp.Service<Resources>().CardHighlight);
            m_quadHighlight.BlendState = new BlendState { ColorSourceBlend = Blend.SourceAlpha, ColorDestinationBlend = Blend.One };

            m_borderBlinkTrack = new Animation.CurveTrack(GameApp.Service<Resources>().CardFloat);
            m_borderBlinkTrack.Loop = true;
            m_borderBlinkTrack.Play();

            m_enlargeTrack = new Animation.CurveTrack(GameApp.Service<Resources>().CardEnlarge);
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
            }
            else if (Control.MouseTracked.MouseEntered != m_lastMouseEntered
                && gameUI.IsCardClickable(Control) && !gameUI.IsCardSelected(Control))
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
            }
            m_lastMouseEntered = Control.MouseTracked.MouseEntered;

            if (m_enlargeTrack.IsPlaying)
            {
                m_enlargeTrack.Elapse(deltaTime);
            }

            m_borderBlinkTrack.Elapse(deltaTime);
        }

        public override void RenderPostMain(Matrix transform, RenderEventArgs e)
        {
            var gameUI = GameApp.Service<Services.GameUI>(); ;
            bool highlightable = gameUI.ZoomedInCard != Control && gameUI.IsCardClickable(Control);

            if (highlightable)
            {
                var xo = (m_quadHighlight.Texture.Width - Control.Region.Width) / 2;
                var yo = (m_quadHighlight.Texture.Height - Control.Region.Height) / 2;
                var region = new Rectangle(Control.Region.Left - xo, Control.Region.Top - yo, m_quadHighlight.Texture.Width, m_quadHighlight.Texture.Height);
                bool selected = gameUI.IsCardSelected(Control);
                Color color;
                if (selected)
                {
                    color = Color.Orange;
                }
                else if (gameUI.ZoomedInCard != Control
                    && gameUI.UIState is Services.UIStates.TacticalPhase
                    && (gameUI.UIState.InteractionObject as Interactions.TacticalPhase).DefenderCandidates.Contains(Control.CardGuid))
                {
                    color = Color.Red;
                }
                else
                {
                    color = Color.Lime;
                }
                m_quadHighlight.ColorToModulate.R = color.R;
                m_quadHighlight.ColorToModulate.G = color.G;
                m_quadHighlight.ColorToModulate.B = color.B;
                m_quadHighlight.ColorToModulate.A = (byte)(selected || Control.MouseTracked.MouseEntered ? 255 : (m_borderBlinkTrack.CurrentValue / 2 + 0.5f) * 255);

                e.RenderManager.Draw(m_quadHighlight, region, transform);
            }
        }

        public bool EvaluateBinding(string id, out string replacement)
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
