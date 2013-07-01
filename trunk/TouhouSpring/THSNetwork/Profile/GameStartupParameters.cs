using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
    public class GameStartupParameters
    {
        public List<string> PlayerIds
        {
            get;
            set;
        }

        public List<Deck> PlayerDecks
        {
            get;
            set;
        }

        public int Seed
        {
            get;
            set;
        }

        public GameStartupParameters()
        {
            Seed = Environment.TickCount;
            PlayerDecks = new List<Deck>();
            PlayerIds = new List<string>();
        }
    }
}
