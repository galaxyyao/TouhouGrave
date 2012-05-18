using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TouhouSpring.Particle.Modifiers
{
	public class RandomPositionInBox : Modifier
	{
		public Vector3 Center { get; set; }
		public Vector3 Dimensions { get; set; }

		protected override void Process(float deltaTime, Particle[] particles, int begin, int end)
		{
			Vector3 halfDim = Dimensions * 0.5f;

			for (int i = begin; i < end; ++i)
			{
				particles[i].m_position = new Vector3
				{
					X = RandomNumber.Generate(-halfDim.X, halfDim.X),
					Y = RandomNumber.Generate(-halfDim.Y, halfDim.Y),
					Z = RandomNumber.Generate(-halfDim.Z, halfDim.Z),
				} + Center;
			}
		}
	}
}
