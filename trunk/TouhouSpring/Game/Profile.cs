using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
    /// <summary>
    /// Describes a game user's preference, like name, decks, etc.
    /// </summary>
    public class Profile
    {
		private List<Deck> m_decks = new List<Deck>();

		/// <summary>
		/// Returns the decks pre-created by the player.
		/// </summary>
        public List<Deck> Decks
        {
            get { return m_decks; }
        }

		/// <summary>
		/// Returns the player's name.
		/// </summary>
		public string Name
		{
			get; set;
		}
    }
}
