using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.UI.CardControlAddins
{
    class ToneAnimation : CardControl.Addin
    {
        private Animation.Track m_saturateTrack;
        private Animation.Track m_brightnessTrack;

        private bool m_lastGrayscale = false;
        private bool m_lastDarken = false;

        public ToneAnimation(CardControl control) : base(control)
        {
            m_saturateTrack = new Animation.LinearTrack(0.5f);
            m_saturateTrack.Elapsed += w => Control.Saturate = w;
            m_saturateTrack.PlayFrom(m_saturateTrack.Duration);
            m_saturateTrack.Stop();

            m_brightnessTrack = new Animation.LinearTrack(0.5f);
            m_brightnessTrack.Elapsed += w => Control.Brightness = w * 0.5f + 0.5f;
            m_brightnessTrack.PlayFrom(m_brightnessTrack.Duration);
            m_brightnessTrack.Stop();
        }

        public override void Update(float deltaTime)
        {
            var gameUI = GameApp.Service<Services.GameUI>();
            bool darken = gameUI.ZoomedInCard != Control
                          && (Card.Behaviors.Has<Behaviors.Warrior>() && Card.Behaviors.Get<Behaviors.Warrior>().State == Behaviors.WarriorState.CoolingDown
                              || Card.Owner.Assists.Contains(Card) && !Card.IsActivatedAssist
                              || Card.Owner.CardsSacrificed.Contains(Card));

            if (darken != m_lastDarken)
            {
                m_brightnessTrack.TimeFactor = darken ? -1 : 1;
                if (!m_brightnessTrack.IsPlaying)
                {
                    m_brightnessTrack.PlayFrom(darken ? m_brightnessTrack.Duration : 0f);
                }
                m_lastDarken = darken;
            }

            var grayscale = gameUI.ZoomedInCard != Control && Card.IsDestroyed;
            if (grayscale != m_lastGrayscale)
            {
                m_saturateTrack.TimeFactor = grayscale ? -1 : 1;
                if (!m_saturateTrack.IsPlaying)
                {
                    m_saturateTrack.PlayFrom(grayscale ? m_saturateTrack.Duration : 0f);
                }
                m_lastGrayscale = grayscale;
            }

            if (m_brightnessTrack.IsPlaying)
            {
                m_brightnessTrack.Elapse(deltaTime);
            }
            if (m_saturateTrack.IsPlaying)
            {
                m_saturateTrack.Elapse(deltaTime);
            }
        }
    }
}
