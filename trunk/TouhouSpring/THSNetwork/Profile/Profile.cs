﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace TouhouSpring
{
    /// <summary>
    /// Describes a game user's preference, like name, decks, etc.
    /// </summary>
    public class Profile
    {
        public string GUID
        {
            get;
            set;
        }

        public string Uid
        {
            get;
            set;
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

        [XmlElement("Decks")]
        public Decks Decks
        {
            get;
            set;
        }

        public Profile()
        {
            Decks = new Decks();
        }
    }
}
