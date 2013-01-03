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
                return m_actingPlayer != -1 ? m_players[m_actingPlayer] : null;
            }
        }

        public IEnumerable<Player> ActingPlayerEnemies
        {
            get
            {
                return m_actingPlayer != -1
                       ? m_players.Where(player => player != m_players[m_actingPlayer])
                       : Enumerable.Empty<Player>();
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
