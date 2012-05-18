using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Particle
{
	public static class RandomNumber
	{
		private static Random s_random = new Random();

		public static int Generate(int minValue, int maxValue)
		{
			return s_random.Next(minValue, maxValue);
		}

		public static float Generate(float minValue, float maxValue)
		{
			return (float)s_random.NextDouble() * (maxValue - minValue) + minValue;
		}
	}
}
