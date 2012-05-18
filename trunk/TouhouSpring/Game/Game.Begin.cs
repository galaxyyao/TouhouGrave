using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
	public partial class Game
	{
		/// <summary>
		/// Call to actually begin a game.
		/// </summary>
		private void Begin()
		{
			// shuffle player's library
			m_players.ForEach(p => ShuffleLibrary(p));

			// TODO: Non-trivial determination of the acting player for the first turn
			m_actingPlayer = 0;

			m_players.ForEach(player => 5.Repeat(i => DrawCard(player)));
		}
	}
}
