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
        public class CardBuild
        {
            public int Id
            {
                get;
                set;
            }

            public List<string> CardModelIds
            {
                get;
                set;
            }

            public List<string> AssistModelIds
            {
                get;
                set;
            }

            public CardBuild(int id)
            {
                Id = id;
                CardModelIds = new List<string>();
                AssistModelIds = new List<string>();
            }
        }

        public List<CardBuild> CardBuilds
        {
            get;
            set;
        }

        public string GUID
        {
            get;
            set;
        }

		public string Uid
		{
			get; set;
		}

        public string Email
        {
            get;
            set;
        }

        public int Deck1Id
        {
            get;
            set;
        }

        public int Deck2Id
        {
            get;
            set;
        }

        public Profile()
        {
            CardBuilds = new List<CardBuild>();
        }
    }
}
