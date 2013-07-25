using System;
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

        public bool Flipped
        {
            get; private set;
        }

        public Flip(CardControl control) : base(control)
        {
            Control.Style.RegisterBinding(this);

            m_track = new Animation.LinearTrack(0.3f);
            m_track.Elapsed += w => UpdateMatrix(Flipped ? w : 1 - w);
        }

        public void InitializeToFlipped()
        {
            UpdateMatrix(1.0f);
            Flipped = true;
        }

        public override void Update(float deltaTime)
        {
            var gameui = GameApp.Service<Services.GameUI>();
            var needFlip = (!gameui.ShallCardBeRevealed(CardData)
                            || CardData.IsTrap && CardData.Zone == SystemZone.Battlefield && !Control.MouseTracked.MouseEntered)
                           && gameui.ZoomedInCard != Control;

            if (needFlip != Flipped)
            {
                if (m_track.IsPlaying)
                {
                    m_track.Stop();
                }

                m_track.Play();
                Flipped = needFlip;
            }

            if (m_track.IsPlaying)
            {
                m_track.Elapse(deltaTime);
            }
        }

        public bool EvaluateBinding(string id, out string replacement)
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

        private void UpdateMatrix(float w)
        {
            float halfWidth = Control.Region.Width * 0.5f;
            m_transform = MatrixHelper.Translate(-halfWidth, 0)
                          * MatrixHelper.Rotate(Vector3.UnitY, MathUtils.PI * w)
                          * MatrixHelper.Translate(halfWidth, 0);
        }
    }
}
