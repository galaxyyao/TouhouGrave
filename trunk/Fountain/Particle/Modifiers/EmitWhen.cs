using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Particle.Modifiers
{
	public class EmitWhen : Modifier
	{
		public string EffectName
		{
			get; set;
		}

		protected override void Process(float deltaTime, Particle[] particles, int begin, int end)
		{
			Effect effect = Effect.System.Effects.FirstOrDefault(fx => fx.Name == EffectName);
			if (effect == null || effect == Effect)
			{
				return;
			}

			for (int i = begin; i < end; ++i)
			{
				if (particles[i].m_position.Z < 0)
				{
					particles[i].m_life = 0; // kill the particle

					var p = effect.Template;
					p.m_position = particles[i].m_position;
					p.m_position.Z = 0;
					effect.Emit(p);
				}
			}

			effect.EndEmit();
		}
	}
}
