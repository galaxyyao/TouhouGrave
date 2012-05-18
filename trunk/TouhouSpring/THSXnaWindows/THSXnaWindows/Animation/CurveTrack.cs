using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XnaCurve = Microsoft.Xna.Framework.Curve;

namespace TouhouSpring.Animation
{
	class CurveTrack : Track
	{
		public XnaCurve Curve
		{
			get; private set;
		}

		public override float Duration
		{
			get { return Curve.Keys.Count == 0 ? 0.0f : Curve.Keys[Curve.Keys.Count - 1].Position; }
		}

		public CurveTrack(XnaCurve curve)
		{
			if (curve == null)
			{
				throw new ArgumentNullException("curve");
			}

			Curve = curve;
		}

		public override float Evaluate(float time)
		{
			return Curve.Evaluate(time);
		}
	}
}
