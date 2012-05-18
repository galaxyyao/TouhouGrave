using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TouhouSpring.Particle.Modifiers
{
	public class RotationAnimation : BaseAnimationModifier
	{
		protected override void ProcessConstant(float constant, float deltaTime, Particle[] particles, int begin, int end)
		{
			for (int i = begin; i < end; ++i)
			{
				particles[i].m_rotation = constant;
			}
		}

		protected override void ProcessLinear(float bias, float scale, float deltaTime, Particle[] particles, int begin, int end)
		{
			for (int i = begin; i < end; ++i)
			{
				particles[i].m_rotation = bias + scale * particles[i].m_age / particles[i].m_life;
			}
		}

		protected override void ProcessCurve(Curve curve, float duration, float deltaTime, Particle[] particles, int begin, int end)
		{
			for (int i = begin; i < end; ++i)
			{
				float t = NormalizeCurveTime ? particles[i].m_age / particles[i].m_life * duration : particles[i].m_age;
				particles[i].m_rotation = CurveObject.Evaluate(t) * CurveScale + CurveBias;
			}
		}
	}
}
