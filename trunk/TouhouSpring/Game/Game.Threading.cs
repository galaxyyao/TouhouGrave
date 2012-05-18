using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace TouhouSpring
{
	public partial class Game
	{
		private Thread m_gameFlowThread;

		/// <summary>
		/// Start a new thread running game flow.
		/// </summary>
		private void StartGameFlowThread()
		{
			if (m_gameFlowThread != null)
			{
				throw new InvalidOperationException("Game flow thread already started.");
			}

            m_gameFlowThread = new Thread(new ThreadStart(Main)) { IsBackground = true };
			m_gameFlowThread.Start();
		}
	}
}
