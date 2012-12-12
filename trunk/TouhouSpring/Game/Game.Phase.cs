using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
	public partial class Game
	{
		private string m_phase = "";

		/// <summary>
		/// Tell the current game phase.
		/// </summary>
		public string CurrentPhase
		{
			get { return m_phase; }
			set
			{
				if (value != m_phase)
				{
					m_phase = value;

					if (InPlayerPhases)
					{
						m_controllers.ForEach(c => c.InternalOnPlayerPhaseChanged(ActingPlayer));
					}
				}
			}
		}

		/// <summary>
		/// Tell whether the game is in player phases, i.e. between PhaseA and PhaseE.
		/// </summary>
		public bool InPlayerPhases
		{
			get; private set;
		}
	}
}
