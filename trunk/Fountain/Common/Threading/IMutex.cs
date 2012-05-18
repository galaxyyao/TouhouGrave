using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Threading
{
	public interface IMutex : IDisposable
	{
		void Lock();
		void Unlock();
	}
}
