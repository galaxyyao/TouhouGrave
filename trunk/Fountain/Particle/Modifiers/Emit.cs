using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Particle.Modifiers
{
	public class Emit : Modifier
	{
		private int m_amount = 0;

		public string EffectName
		{
			get; set;
		}

		public int Amount
		{
			get { return m_amount; }
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("Amount should be non-negative.");
				}

				m_amount = value;
			}
		}

		protected override void Process(float deltaTime, Particle[] particles, int begin, int end)
		{
			Effect effect = Effect.System.Effects.FirstOrDefault(fx => fx.Name == EffectName);
			if (effect == null || effect == Effect || Amount <= 0)
			{
				return;
			}

			for (int i = 0; i < Amount * Math.Max(end - begin, 0); ++i)
			{
				effect.Emit(effect.Template);
			}
			effect.EndEmit();
		}
	}
}
