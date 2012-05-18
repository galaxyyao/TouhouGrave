using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TouhouSpring.Particle.Modifiers
{
	public class ColorAnimation : BaseAnimationModifier
	{
		public bool AffectRed { get; set; }
		public bool AffectGreen { get; set; }
		public bool AffectBlue { get; set; }
		public bool AffectAlpha { get; set; }

		public override float StartValue
		{
			get { return base.StartValue; }
			set
			{
				if (value < 0f || value > 1f)
				{
					throw new ArgumentOutOfRangeException("Value should be in range [0, 1].");
				}
				base.StartValue = value;
			}
		}

		public override float FinishValue
		{
			get { return base.FinishValue; }
			set
			{
				if (value < 0f || value > 1f)
				{
					throw new ArgumentOutOfRangeException("Value should be in range [0, 1].");
				}
				base.FinishValue = value;
			}
		}

		public ColorAnimation()
		{
			AffectRed = true;
			AffectBlue = true;
			AffectGreen = true;
			AffectAlpha = true;
		}

		protected override void ProcessConstant(float constant, float deltaTime, Particle[] particles, int begin, int end)
		{
			UInt32 mask = GetMask();
			int multiplier = GetMultiplier();

			var s = (UInt32)((byte)(constant * 255) * multiplier);
			for (int i = begin; i < end; ++i)
			{
				particles[i].m_color.underlay = (particles[i].m_color.underlay & mask) | s;
			}
		}

		protected override void ProcessLinear(float bias, float scale, float deltaTime, Particle[] particles, int begin, int end)
		{
			UInt32 mask = GetMask();
			int multiplier = GetMultiplier();

			for (int i = begin; i < end; ++i)
			{
				var c = (byte)((bias + scale * particles[i].m_age / particles[i].m_life) * 255);
				var s = (UInt32)(c * multiplier);
				particles[i].m_color.underlay = (particles[i].m_color.underlay & mask) | s;
			}
		}

		protected override void ProcessCurve(Curve curve, float duration, float deltaTime, Particle[] particles, int begin, int end)
		{
			UInt32 mask = GetMask();
			int multiplier = GetMultiplier();

			for (int i = begin; i < end; ++i)
			{
				float w = NormalizeCurveTime ? particles[i].m_age / particles[i].m_life * duration : particles[i].m_age;
				var c = (byte)((CurveObject.Evaluate(w) * CurveScale + CurveBias) * 255);
				var s = (UInt32)(c * multiplier);
				particles[i].m_color.underlay = (particles[i].m_color.underlay & mask) | s;
			}
		}

		private UInt32 GetMask()
		{
			UInt32 mask = 0xffffffff;

			if (AffectRed) { mask &= ~Color.MaskR; }
			if (AffectGreen) { mask &= ~Color.MaskG; }
			if (AffectBlue) { mask &= ~Color.MaskB; }
			if (AffectAlpha) { mask &= ~Color.MaskA; }

			return mask;
		}

		private int GetMultiplier()
		{
			int multiplier = 0;

			if (AffectRed) { multiplier += 0x1 << Color.ShiftR; }
			if (AffectGreen) { multiplier += 0x1 << Color.ShiftG; }
			if (AffectBlue) { multiplier += 0x1 << Color.ShiftB; }
			if (AffectAlpha) { multiplier += 0x1 << Color.ShiftA; }

			return multiplier;
		}
	}
}
