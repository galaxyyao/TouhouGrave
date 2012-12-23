using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Particle
{
	public class Modifier
	{
		internal void InternalProcess(float deltaTime, EffectInstance instance, Particle[] particles, int begin, int end)
		{
			Process(deltaTime, instance, particles, begin, end);
		}

        protected virtual void Process(float deltaTime, EffectInstance instance, Particle[] particles, int begin, int end) { }
	}
}
