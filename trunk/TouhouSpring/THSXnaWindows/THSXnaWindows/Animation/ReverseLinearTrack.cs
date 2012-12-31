using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Animation
{
	class ReverseLinearTrack : Track
	{
		private float m_duraion;

		public override float Duration
		{
			get { return m_duraion; }
		}

        public ReverseLinearTrack(float duration)
		{
			if (duration == 0.0f)
			{
				throw new ArgumentOutOfRangeException("Duration can't be zero.");
			}

			m_duraion = duration;
		}

		public override float Evaluate(float time)
		{
            return 1.0f - time / Duration;
		}
	}
}
