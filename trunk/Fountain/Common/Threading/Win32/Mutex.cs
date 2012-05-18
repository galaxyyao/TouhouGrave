using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Threading.Win32
{
	class Mutex :  IMutex
	{
		private System.Threading.Mutex m_mutex = new System.Threading.Mutex();

		public void Dispose()
		{
			m_mutex.Close();
			m_mutex = null;
		}

		public void Lock()
		{
			m_mutex.WaitOne();
		}

		public void Unlock()
		{
			m_mutex.ReleaseMutex();
		}
	}
}
