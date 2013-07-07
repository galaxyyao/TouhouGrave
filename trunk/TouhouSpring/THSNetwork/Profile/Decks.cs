using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace TouhouSpring
{
    public class Decks
    {
        [XmlElement("Deck")]
        public List<Deck> MyDecks
        {
            get;
            set;
        }

        public Decks()
        {
            MyDecks = new List<Deck>();
        }

        public int MaxDeckId
        {
            get
            {
                return (from deck in MyDecks
                        select deck).Max<Deck>(deck => deck.Id);
            }
        }
    }
}
