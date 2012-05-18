using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TouhouSpring.Particle.Modifiers
{
	public class Accelerate : Modifier
	{
		public Vector3 Acceleration { get; set; }

		protected override void Process(float deltaTime, Particle[] particles, int begin, int end)
		{
			var v = Acceleration * deltaTime;
			for (int i = begin; i < end; ++i)
			{
				particles[i].m_velocity += v;
			}
		}
	}
}
