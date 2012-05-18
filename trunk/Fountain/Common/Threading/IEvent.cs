using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Threading
{
	public interface IEvent : IDisposable
	{
		void Reset();
		void Singal();
		void Wait();
	}
}
