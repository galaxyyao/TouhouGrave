using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TouhouSpring.Particle.Modifiers
{
	public class Fan : Modifier
	{
		public Vector3 Center { get; set; }
		public Vector3 XAxis { get; set; }
		public Vector3 YAxis { get; set; }
		public float AngularOffset { get; set; }
		public float AngularInterval { get; set; }

		private float m_phase = 0;

        protected override void Process(float deltaTime, EffectInstance instance, Particle[] particles, int begin, int end)
		{
			for (int i = begin; i < end; ++i)
			{
				m_phase = (m_phase + AngularInterval) % 360.0f;

				float sin = (float)Math.Sin(MathHelper.ToRadians(m_phase));
				float cos = (float)Math.Cos(MathHelper.ToRadians(m_phase));

				particles[i].m_position = Center + XAxis * sin + YAxis * cos;
				particles[i].m_rotation = m_phase + AngularOffset;
			}
		}
	}
}
