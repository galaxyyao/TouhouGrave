using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Threading.Win32
{
	class Event : IEvent
	{
		private System.Threading.EventWaitHandle m_event;

		public Event(bool autoReset, bool signaled)
		{
			if (autoReset)
			{
				m_event = new System.Threading.AutoResetEvent(signaled);
			}
			else
			{
				m_event = new System.Threading.ManualResetEvent(signaled);
			}
		}

		public void Dispose()
		{
			m_event.Close();
			m_event = null;
		}

		public void Reset()
		{
			m_event.Reset();
		}

		public void Singal()
		{
			m_event.Set();
		}

		public void Wait()
		{
			m_event.WaitOne();
		}
	}
}
