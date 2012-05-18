using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TouhouSpring.Particle.Modifiers
{
	public abstract class BaseAnimationModifier : Modifier
	{
		private AnimationType m_animType = AnimationType.Constant;
		private string m_curveName = null;
		private float m_startValue = 0f;
		private float m_finishValue = 0f;

		public AnimationType AnimationType
		{
			get { return m_animType; }
			set
			{
				if (value != m_animType)
				{
					if (m_animType == AnimationType.Curve)
					{
						CurveObject = null;
					}
					else if (value == AnimationType.Curve)
					{
						CurveObject = ResourceLoader.Instance.LoadCurve(m_curveName);
					}
					m_animType = value;
				}
			}
		}

#if WINDOWS
		[System.ComponentModel.TypeConverter(typeof(CurveNameTypeConverter))]
#endif
		public string CurveName
		{
			get { return m_curveName; }
			set
			{
				if (value != m_curveName)
				{
					CurveObject = value != null ? ResourceLoader.Instance.LoadCurve(value) : null;
					m_curveName = value;
				}
			}
		}

		public bool NormalizeCurveTime
		{
			get; set;
		}

		public float CurveScale
		{
			get; set;
		}

		public float CurveBias
		{
			get; set;
		}

#if WINDOWS
		[System.ComponentModel.Browsable(false)]
#endif
		public Curve CurveObject
		{
			get; private set;
		}

		public virtual float StartValue
		{
			get { return m_startValue; }
			set { m_startValue = value; }
		}

		public virtual float FinishValue
		{
			get { return m_finishValue; }
			set { m_finishValue = value; }
		}

		public BaseAnimationModifier()
		{
			NormalizeCurveTime = true;
			CurveScale = 1f;
			CurveBias = 0f;
		}

		protected override void Process(float deltaTime, Particle[] particles, int begin, int end)
		{
			if (AnimationType == Modifiers.AnimationType.Constant
				|| AnimationType == Modifiers.AnimationType.Curve && CurveObject == null)
			{
				ProcessConstant(m_startValue, deltaTime, particles, begin, end);
			}
			else if (AnimationType == Modifiers.AnimationType.Linear)
			{
				ProcessLinear(m_startValue, m_finishValue - m_startValue, deltaTime, particles, begin, end);
			}
			else
			{
				if (CurveObject.Keys.Count == 0)
				{
					return;
				}
				float duration = CurveObject.Keys.Last().Position;
				ProcessCurve(CurveObject, duration, deltaTime, particles, begin, end);
			}
		}

		protected abstract void ProcessConstant(float constant, float deltaTime, Particle[] particles, int begin, int end);
		protected abstract void ProcessLinear(float bias, float scale, float deltaTime, Particle[] particles, int begin, int end);
		protected abstract void ProcessCurve(Curve curve, float duration, float deltaTime, Particle[] particles, int begin, int end);
	}
}
