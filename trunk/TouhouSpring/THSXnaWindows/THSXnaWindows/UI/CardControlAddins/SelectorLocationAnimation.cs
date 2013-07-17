using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TouhouSpring.UI.CardControlAddins
{
    class SelectorLocationAnimation : CardControl.Addin, Style.IBindingProvider
    {
        public interface IWindow
        {
            int Center { get; }
            Matrix GetLocation(int index);
        }

        private IWindow m_window;
        private int m_lastCenter;

        private Animation.Track m_locationTrack;
        private Matrix m_locationSrcTransform;
        private Matrix m_locationDstTransform;
        private Matrix m_locationTransform = MatrixHelper.Identity;

        public int LocationIndex
        {
            get; private set;
        }

        public SelectorLocationAnimation(CardControl control, IWindow window)
            : base(control)
        {
            if (window == null)
            {
                throw new ArgumentNullException("locationResolver");
            }

            m_window = window;

            m_locationTrack = new Animation.CurveTrack(GameApp.Service<Services.ResourceManager>().Acquire<Curve>("Curves/CardMove"));
            m_locationTrack.Elapsed += w =>
            {
                m_locationTransform = Matrix.Lerp(m_locationSrcTransform, m_locationDstTransform, w);
            };

            Control.Style.RegisterBinding(this);
        }

        public void ResetLocationIndex(int index)
        {
            LocationIndex = index;
            m_lastCenter = m_window.Center;
            m_locationSrcTransform = m_window.GetLocation(index);
            m_locationTransform = m_locationSrcTransform;
            if (m_locationTrack.IsPlaying)
            {
                m_locationTrack.Stop();
            }
        }

        public override void Update(float deltaTime)
        {
            if (m_window.Center != m_lastCenter)
            {
                if (m_locationTrack.IsPlaying)
                {
                    m_locationTrack.Stop();
                }

                m_locationSrcTransform = Control.Transform;
                m_locationDstTransform = m_window.GetLocation(LocationIndex);
                m_locationTrack.Play();

                m_lastCenter = m_window.Center;
            }

            if (m_locationTrack.IsPlaying)
            {
                m_locationTrack.Elapse(deltaTime);
            }
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
    }
}
