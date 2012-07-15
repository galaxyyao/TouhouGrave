﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TouhouSpring.UI.CardControlAddins
{
    class Flip : CardControl.Addin, Style.IBindingProvider
    {
        private Animation.Track m_track;
        private Matrix m_transform = Matrix.Identity;
        private bool m_lastFlipped = false;

        public Flip(CardControl control) : base(control)
        {
            Control.Style.RegisterBinding(this);

            m_track = new Animation.LinearTrack(0.3f);
            m_track.Elapsed += w =>
            {
                w = m_lastFlipped ? w : 1 - w;
                var halfWidth = 0.5f;
                m_transform = MatrixHelper.Translate(-halfWidth, 0)
                              * MatrixHelper.Rotate(Vector3.UnitY, MathUtils.PI * w)
                              * MatrixHelper.Translate(halfWidth, 0);
            };
        }

        public override void Update(float deltaTime)
        {
            var needFlip = GameApp.Service<Services.GameUI>().ZoomedInCard != Control
                           && Card.Behaviors.Has<Behaviors.Warrior>()
                           && Card.Behaviors.Get<Behaviors.Warrior>().State == Behaviors.WarriorState.CoolingDown;

            if (needFlip != m_lastFlipped)
            {
                if (m_track.IsPlaying)
                {
                    m_track.Stop();
                }

                m_track.Play();
                m_lastFlipped = needFlip;
            }

            if (m_track.IsPlaying)
            {
                m_track.Elapse(deltaTime);
            }
        }

        public bool TryGetValue(string id, out string replacement)
        {
            switch (id)
            {
                case "CardAnimator.Flip":
                    replacement = m_transform.Serialize();
                    return true;

                default:
                    replacement = null;
                    return false;
            }
        }
    }
}
