using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
	public partial class Game
	{
		private Profile[] m_profiles;
		private Player[] m_players;

		private int m_actingPlayer = -1;

		/// <summary>
		/// Give the current player's Player object.
		/// </summary>
		public Player PlayerPlayer
		{
			get
			{
				if (!InPlayerPhases)
				{
					throw new InvalidOperationException("ActingPlayer can only be evaluated in one of the five player phases.");
				}
				return m_players[m_actingPlayer];
			}
		}

		/// <summary>
		/// Give the opponent player's Player object.
		/// TODO: support game among more than 2 players
		/// </summary>
		public Player OpponentPlayer
		{
			get
			{
				if (!InPlayerPhases)
				{
					throw new InvalidOperationException("OpponentPlayer can only be evaluated in one of the five player phases.");
				}
				return m_players[1 - m_actingPlayer];
			}
		}

		/// <summary>
		/// Return a collection of all Player objects.
		/// </summary>
		public IIndexable<Player> Players
		{
			get { return m_players.ToIndexable(); }
		}
	}
}
