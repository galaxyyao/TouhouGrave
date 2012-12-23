using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TouhouSpring.Particle.Modifiers
{
	public class RandomVelocityInSphere : Modifier
	{
		public float Radius { get; set; }

		protected override void Process(float deltaTime, EffectInstance instance, Particle[] particles, int begin, int end)
		{
			for (int i = begin; i < end; ++i)
			{
				float r3 = RandomNumber.Generate(0, Radius * Radius * Radius);
				float r = (float)Math.Pow(r3, 1.0 / 3.0);

				float theta = RandomNumber.Generate(0, MathHelper.TwoPi);
				float sinTheta = (float)Math.Sin(theta);
				float cosTheta = (float)Math.Cos(theta);

				float phi = RandomNumber.Generate(-MathHelper.PiOver2, MathHelper.PiOver2);
				float sinPhi = (float)Math.Sin(phi);
				float cosPhi = (float)Math.Cos(phi);

				particles[i].m_velocity = new Vector3
				{
					X = r * cosTheta * cosPhi,
					Y = r * sinPhi,
					Z = r * sinTheta * cosPhi
				};
			}
		}
	}
}
