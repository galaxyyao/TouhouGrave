using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TouhouSpring.Particle.Modifiers
{
	public class Explosion : Modifier
	{
		public Vector3 Center { get; set; }
		public float Strength { get; set; }
		public bool Accelerate { get; set; }

		public Explosion()
		{
			Strength = 1f;
		}

        protected override void Process(float deltaTime, EffectInstance instance, Particle[] particles, int begin, int end)
		{
			for (int i = begin; i < end; ++i)
			{
				Vector3 d = particles[i].m_position - Center;
				particles[i].m_velocity = (Accelerate ? d : Vector3.Normalize(d)) * Strength;
			}
		}
	}
}
