using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TouhouSpring.UI.CardControlAddins
{
    class LocationAnimation : CardControl.Addin, Style.IBindingProvider, Particle.ILocalFrameProvider
    {
        public struct LocationParameter
        {
            public Services.GameUI.CardZone m_zone;
            public int m_numCards;
            public int m_thisIndex;
            public int m_focusIndex;
        }

        public LocationParameter NextLocation;

        private Animation.Track m_locationTrack;
        private Matrix m_locationSrcTransform;
        private Matrix m_locationDstTransform;
        private Matrix m_locationTransform = MatrixHelper.Identity;
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

        public LocationAnimation(CardControl control) : base(control)
        {
            Control.Style.RegisterBinding(this);

            m_locationTrack = new Animation.CurveTrack(GameApp.Service<Services.ResourceManager>().Acquire<Curve>("Curves/CardMove"));
            m_locationTrack.Elapsed += w =>
            {
                m_locationTransform = Matrix.Lerp(m_locationSrcTransform, m_locationDstTransform, w);
                m_cardSummoned.EffectInstances.ForEach(fx => fx.IsEmitting = m_playToBattlefield && w != 1.0f);
            };

            NextLocation = m_lastLocation = new LocationParameter { m_zone = null, m_numCards = 0, m_thisIndex = -1, m_focusIndex = -1 };

            m_cardSummoned = new Particle.ParticleSystemInstance(GameApp.Service<Services.ResourceManager>().Acquire<Particle.ParticleSystem>("Particles/CardSummoned"));
            m_cardSummoned.LocalFrameProvider = this;

            m_cardActivated = new Particle.ParticleSystemInstance(GameApp.Service<Services.ResourceManager>().Acquire<Particle.ParticleSystem>("Particles/CardActivated"));
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
                m_locationDstTransform = NextLocation.m_zone.ResolveLocationTransform(Control, NextLocation.m_thisIndex);
                m_locationTrack.Play();

                m_playToBattlefield = (NextLocation.m_zone.ZoneId == "Battlefield"
                                       || NextLocation.m_zone.ZoneId == "Sacrifice"
                                       || NextLocation.m_zone.ZoneId == "Hero")
                                      && m_lastLocation.m_zone != null
                                      && m_lastLocation.m_zone.ZoneId == "Hand";
                m_lastLocation = NextLocation;
            }

            var transform = Matrix.CreateTranslation(Control.Region.Width * 0.5f, -Control.Region.Height * 0.5f, 0)
                            * Matrix.CreateScale(1, -1, 1)
                            * Control.BodyContainer.TransformToGlobal;
            m_localFrame.Col0 = new Vector4(transform.M11, transform.M21, transform.M31, transform.M41);
            m_localFrame.Col1 = new Vector4(transform.M12, transform.M22, transform.M32, transform.M42);
            m_localFrame.Col2 = new Vector4(transform.M13, transform.M23, transform.M33, transform.M43);
            m_localFrame.Col3 = new Vector4(transform.M14, transform.M24, transform.M34, transform.M44);

            bool activated = CardData.IsAssistActivated;
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

        public bool EvaluateBinding(string id, out string replacement)
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

        public void SetNextLocation(Services.GameUI.CardZone nextZone, int sortIndex)
        {
            NextLocation.m_zone = nextZone;
            NextLocation.m_thisIndex = sortIndex;
            Control.EnableDepth = nextZone.EnableDepth;

            if (Control.Dispatcher != nextZone.Container)
            {
                Control.SetParentAndKeepPosition(nextZone.Container);
                m_locationTransform = Control.Transform;
            }
        }

        public Particle.LocalFrame LocalFrame
        {
            get { return m_localFrame; }
        }
    }
}
