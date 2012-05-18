using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Particle.Modifiers
{
	public class Switch : Modifier
	{
		public string EffectName
		{
			get; set;
		}

		public float Duration { get; set; }

		protected override void Process(float deltaTime, Particle[] particles, int begin, int end)
		{
			Effect effect = Effect.System.Effects.FirstOrDefault(fx => fx.Name == EffectName);
			if (effect == null || effect == Effect)
			{
				return;
			}

			if (end > begin)
			{
				effect.IsEmitting = particles[begin].m_age <= Duration;
			}
		}
	}
}
