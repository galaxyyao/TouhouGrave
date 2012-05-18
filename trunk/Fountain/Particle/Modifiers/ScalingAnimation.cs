using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TouhouSpring.Particle.Modifiers
{
	public class ScalingAnimation : BaseAnimationModifier
	{
		public bool AffectWidth { get; set; }
		public bool AffectHeight { get; set; }

		public override float StartValue
		{
			get { return base.StartValue; }
			set
			{
				if (value < 0.0f)
				{
					throw new ArgumentOutOfRangeException("Value should be non-negative.");
				}
				base.StartValue = value;
			}
		}

		public override float FinishValue
		{
			get { return base.FinishValue; }
			set
			{
				if (value < 0.0f)
				{
					throw new ArgumentOutOfRangeException("Value should be non-negative.");
				}
				base.FinishValue = value;
			}
		}

		public ScalingAnimation()
		{
			AffectWidth = true;
			AffectHeight = true;
		}

		protected override void ProcessConstant(float constant, float deltaTime, Particle[] particles, int begin, int end)
		{
			if (AffectWidth && AffectHeight)
			{
				for (int i = begin; i < end; ++i)
				{
					particles[i].m_size = new Vector2(constant, constant);
				}
			}
			else if (AffectWidth)
			{
				for (int i = begin; i < end; ++i)
				{
					particles[i].m_size.X = constant;
				}
			}
			else if (AffectHeight)
			{
				for (int i = begin; i < end; ++i)
				{
					particles[i].m_size.Y = constant;
				}
			}
		}

		protected override void ProcessLinear(float bias, float scale, float deltaTime, Particle[] particles, int begin, int end)
		{
			if (AffectWidth && AffectHeight)
			{
				for (int i = begin; i < end; ++i)
				{
					particles[i].m_size = new Vector2(bias + scale * particles[i].m_age / particles[i].m_life);
				}
			}
			else if (AffectWidth)
			{
				for (int i = begin; i < end; ++i)
				{
					particles[i].m_size.X = bias + scale * particles[i].m_age / particles[i].m_life;
				}
			}
			else if (AffectHeight)
			{
				for (int i = begin; i < end; ++i)
				{
					particles[i].m_size.Y = bias + scale * particles[i].m_age / particles[i].m_life;
				}
			}
		}

		protected override void ProcessCurve(Curve curve, float duration, float deltaTime, Particle[] particles, int begin, int end)
		{
			if (AffectWidth && AffectHeight)
			{
				for (int i = begin; i < end; ++i)
				{
					float t = NormalizeCurveTime ? particles[i].m_age / particles[i].m_life * duration : particles[i].m_age;
					particles[i].m_size = new Vector2(CurveObject.Evaluate(t) * CurveScale + CurveBias);
				}
			}
			else if (AffectWidth)
			{
				for (int i = begin; i < end; ++i)
				{
					float t = NormalizeCurveTime ? particles[i].m_age / particles[i].m_life * duration : particles[i].m_age;
					particles[i].m_size.X = CurveObject.Evaluate(t) * CurveScale + CurveBias;
				}
			}
			else if (AffectHeight)
			{
				for (int i = begin; i < end; ++i)
				{
					float t = NormalizeCurveTime ? particles[i].m_age / particles[i].m_life * duration : particles[i].m_age;
					particles[i].m_size.Y = CurveObject.Evaluate(t) * CurveScale + CurveBias;
				}
			}
		}
	}
}
