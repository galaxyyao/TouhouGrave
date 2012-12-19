using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TouhouSpring.UI.CardControlAddins
{
	class LocationAnimation : CardControl.Addin, Style.IBindingProvider
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
			};

            m_locationTransformResolver = locationResolver;
            NextLocation = m_lastLocation = new LocationParameter { m_zone = null, m_numCards = 0, m_thisIndex = -1, m_focusIndex = -1 };
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

                m_lastLocation = NextLocation;
            }

			if (m_locationTrack.IsPlaying)
			{
				m_locationTrack.Elapse(deltaTime);
			}
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
	}
}
