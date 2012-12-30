using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TouhouSpring.UI.CardControlAddins
{
    class LocationAnimation : CardControl.Addin, Style.IBindingProvider, Particle.ILocalFrameProvider
    {
        public delegate Matrix LocationTransformResolver(LocationParameter location, CardControl control);

        public class ZoneInfo
        {
            public UI.EventDispatcher m_container;
            public float m_width;
            public int[] m_intervalReductionLevel;
            public float[] m_intervalReductionAmount;
            public int m_lastFocusIndex; // focus index will be locked during card location transition
                                         // so that glitching in transition animation could be avoided
        }

        public struct LocationParameter
        {
            public ZoneInfo m_zone;
            public int m_numCards;
            public int m_thisIndex;
            public int m_focusIndex;
        }

        public LocationParameter NextLocation;

        private Animation.Track m_locationTrack;
        private Matrix m_locationSrcTransform;
        private Matrix m_locationDstTransform;
        private Matrix m_locationTransform = MatrixHelper.Identity;
        private LocationTransformResolver m_locationTransformResolver;
        private LocationParameter m_lastLocation;

        private bool m_playToBattlefield;
        private bool m_lastActivated;
        private Particle.ParticleSystemInstance m_cardSummoned;
        private Particle.ParticleSystemInstance m_cardActivated;
        private Animation.Track m_activateEffectTimer;
        private Particle.LocalFrame m_localFrame;

        public bool InTransition
        {
            get { return m_locationTrack.IsPlaying; }
        }

        public LocationAnimation(CardControl control, LocationTransformResolver locationResolver) : base(control)
        {
            Control.Style.RegisterBinding(this);

            m_locationTrack = new Animation.CurveTrack(GameApp.Service<Services.ResourceManager>().Acquire<Curve>("Curve_CardMove"));
            m_locationTrack.Elapsed += w =>
            {
                m_locationTransform = Matrix.Lerp(m_locationSrcTransform, m_locationDstTransform, w);
                m_cardSummoned.EffectInstances.ForEach(fx => fx.IsEmitting = m_playToBattlefield && w != 1.0f);
            };

            m_locationTransformResolver = locationResolver;
            NextLocation = m_lastLocation = new LocationParameter { m_zone = null, m_numCards = 0, m_thisIndex = -1, m_focusIndex = -1 };

            m_cardSummoned = new Particle.ParticleSystemInstance(GameApp.Service<Services.ResourceManager>().Acquire<Particle.ParticleSystem>("CardSummoned"));
            m_cardSummoned.LocalFrameProvider = this;

            m_cardActivated = new Particle.ParticleSystemInstance(GameApp.Service<Services.ResourceManager>().Acquire<Particle.ParticleSystem>("CardActivated"));
            m_cardActivated.LocalFrameProvider = this;
            m_activateEffectTimer = new Animation.LinearTrack(0.4f);
            m_activateEffectTimer.Elapsed += w =>
            {
                m_cardActivated.EffectInstances.ForEach(fx => fx.IsEmitting = m_lastActivated && w != 1.0f);
            };
            m_activateEffectTimer.Play();
        }

        public override void Update(float deltaTime)
        {
            if (NextLocation.m_zone != m_lastLocation.m_zone
                || NextLocation.m_numCards != m_lastLocation.m_numCards
                || NextLocation.m_thisIndex != m_lastLocation.m_thisIndex
                || NextLocation.m_focusIndex != m_lastLocation.m_focusIndex)
            {
                if (m_locationTrack.IsPlaying)
                {
                    m_locationTrack.Stop();
                }

                m_locationSrcTransform = Control.Transform;
                m_locationDstTransform = m_locationTransformResolver(NextLocation, Control);
                m_locationTrack.Play();

                var gameui = GameApp.Service<Services.GameUI>();
                var playerHand = gameui.InGameUIPage.Style.ChildIds["PlayerHand"].Target;
                var playerBattlefield = gameui.InGameUIPage.Style.ChildIds["PlayerBattlefield"].Target;
                var playerHero = gameui.InGameUIPage.Style.ChildIds["PlayerHero"].Target;
                var playerSacrifice = gameui.InGameUIPage.Style.ChildIds["PlayerSacrifice"].Target;
                var opponentHand = gameui.InGameUIPage.Style.ChildIds["OpponentHand"].Target;
                var opponentBattlefield = gameui.InGameUIPage.Style.ChildIds["OpponentBattlefield"].Target;
                var opponentHero = gameui.InGameUIPage.Style.ChildIds["OpponentHero"].Target;
                var opponentSacrifice = gameui.InGameUIPage.Style.ChildIds["OpponentSacrifice"].Target;

                m_playToBattlefield = (NextLocation.m_zone.m_container == playerBattlefield
                                       || NextLocation.m_zone.m_container == playerSacrifice
                                       || NextLocation.m_zone.m_container == playerHero)
                                      && m_lastLocation.m_zone.m_container == playerHand
                                      || (NextLocation.m_zone.m_container == opponentBattlefield
                                          || NextLocation.m_zone.m_container == opponentSacrifice
                                          || NextLocation.m_zone.m_container == opponentHero)
                                         && m_lastLocation.m_zone.m_container == opponentHand;
                m_lastLocation = NextLocation;
            }

            var transform = Matrix.CreateTranslation(Control.Region.Width * 0.5f, -Control.Region.Height * 0.5f, 0)
                            * Matrix.CreateScale(1, -1, 1)
                            * Control.Style.MainLayout.TransformToGlobal;
            m_localFrame.Col0 = new Vector4(transform.M11, transform.M21, transform.M31, transform.M41);
            m_localFrame.Col1 = new Vector4(transform.M12, transform.M22, transform.M32, transform.M42);
            m_localFrame.Col2 = new Vector4(transform.M13, transform.M23, transform.M33, transform.M43);
            m_localFrame.Col3 = new Vector4(transform.M14, transform.M24, transform.M34, transform.M44);

            bool activated = Card.IsAssist && Card.Owner.ActivatedAssist == Card;
            if (activated != m_lastActivated)
            {
                if (m_activateEffectTimer.IsPlaying)
                {
                    m_activateEffectTimer.Stop();
                    m_cardActivated.EffectInstances.ForEach(fx => fx.IsEmitting = false);
                }

                if (activated)
                {
                    m_activateEffectTimer.Play();
                    m_cardActivated.EffectInstances.ForEach(fx => fx.IsEmitting = true);
                    for (int i = 0; i < 2; ++i)
                    {
                        m_cardActivated.Update(1f);
                    }
                }

                m_lastActivated = activated;
            }

            if (m_locationTrack.IsPlaying)
            {
                m_locationTrack.Elapse(deltaTime);
            }
            if (m_activateEffectTimer.IsPlaying)
            {
                m_activateEffectTimer.Elapse(deltaTime);
            }

            m_cardSummoned.Update(deltaTime);
            m_cardActivated.Update(deltaTime);
        }

        public override void RenderPostMain(Matrix transform, RenderEventArgs e)
        {
            GameApp.Service<Graphics.ParticleRenderer>().Draw(m_cardSummoned, Matrix.Identity, 1.0f, 1.3333f);
            GameApp.Service<Graphics.ParticleRenderer>().Draw(m_cardActivated, Matrix.Identity, 1.0f, 1.3333f);
        }

        public bool TryGetValue(string id, out string replacement)
        {
            switch (id)
            {
                case "CardAnimator.LocationTransform":
                    replacement = m_locationTransform.Serialize();
                    return true;

                default:
                    replacement = null;
                    return false;
            }
        }

        public void OnLocationSet()
        {
            if (Control.Dispatcher != NextLocation.m_zone.m_container)
            {
                Control.SetParentAndKeepPosition(NextLocation.m_zone.m_container);
                m_locationTransform = Control.Transform;
            }
        }

        public Particle.LocalFrame LocalFrame
        {
            get { return m_localFrame; }
        }
    }
}
