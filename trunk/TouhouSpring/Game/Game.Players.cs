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
        private BaseController[] m_controllers;

        private int m_actingPlayer = -1;

        /// <summary>
        /// Give the current player's Player object.
        /// </summary>
        public Player ActingPlayer
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

        public IEnumerable<Player> ActingPlayerEnemies
        {
            get
            {
                if (!InPlayerPhases)
                {
                    throw new InvalidOperationException("ActingPlayerEnemies can only be evaluated in one of the five player phases.");
                }
                return m_players.Where(player => player != m_players[m_actingPlayer]);
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
