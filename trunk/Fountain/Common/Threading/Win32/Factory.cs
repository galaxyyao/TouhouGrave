using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Threading.Win32
{
	class Factory : IFactory
	{
		public IEvent NewEvent(bool autoReset, bool signaled)
		{
			return new Event(autoReset, signaled);
		}

		public IMutex NewMutex()
		{
			return new Mutex();
		}
	}
}
