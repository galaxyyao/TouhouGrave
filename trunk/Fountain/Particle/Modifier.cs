using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Particle
{
	public class Modifier
	{
#if WINDOWS
		[System.ComponentModel.Browsable(false)]
#endif
		public Effect Effect
		{
			get; internal set;
		}

		internal void InternalProcess(float deltaTime, Particle[] particles, int begin, int end)
		{
			Process(deltaTime, particles, begin, end);
		}

		protected virtual void Process(float deltaTime, Particle[] particles, int begin, int end) { }
	}
}
