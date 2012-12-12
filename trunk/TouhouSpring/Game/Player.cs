using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
    /// <summary>
    /// Represent a collection of player-specific states.
    /// </summary>
    public class Player
    {
        private Profile m_profile;

        internal List<BaseCard> m_handSet = new List<BaseCard>();
        internal List<BaseCard> m_battlefieldCards = new List<BaseCard>();
        internal Pile m_library = new Pile();
        internal Pile m_graveyard = new Pile();
        private const int m_initialManaDelta = 2;

        /// <summary>
        /// Return a collection of cards on hand.
        /// </summary>
        public IIndexable<BaseCard> CardsOnHand
        {
            get { return m_handSet.ToIndexable(); }
        }

        /// <summary>
        /// Return a collection of cards on battlefield.
        /// </summary>
        public IIndexable<BaseCard> CardsOnBattlefield
        {
            get { return m_battlefieldCards.ToIndexable(); }
        }

        public string Name
        {
            get { return m_profile.Name; }
        }

        public int Health
        {
            get; internal set;
        }

        public int ReservedHealth
        {
            get; internal set;
        }

        public int FreeHealth
        {
            get { return Health - ReservedHealth; }
        }

        public int Mana
        {
            get; internal set;
        }

        public int ReservedMana
        {
            get; internal set;
        }

        public int FreeMana
        {
            get { return Mana - ReservedMana; }
        }

        public int ManaDelta
        {
            get; set;
        }

        public bool IsSkillCharged
        {
            get; internal set;
        }

        public Behaviors.Hero Hero
        {
            get; private set;
        }

        internal Player(Profile profile)
        {
            if (profile == null)
            {
                throw new ArgumentNullException("profile");
            }

            m_profile = profile;
        }

        /// <summary>
        /// Initialize a player's states.
        /// </summary>
        /// <param name="deck">The deck from which a library pile is generated.</param>
        /// <param name="heroCard">The hero used by the player.</param>
        internal void Initialize(Deck deck, ICardModel heroCard)
        {
            if (deck == null)
            {
                throw new ArgumentNullException("deck");
            }
            else if (heroCard == null)
            {
                throw new ArgumentNullException("heroCard");
            }
            else if (!m_profile.Decks.Contains(deck))
            {
                throw new ArgumentOutOfRangeException("deck");
            }

            // initialize player's library
            foreach (var cardModel in deck)
            {
                m_library.AddCardToTop(new BaseCard(cardModel, this));
            }

            // initialize player's hero
            BaseCard hero = new BaseCard(heroCard, this);
            var heroBhv = hero.Behaviors.Get<Behaviors.Hero>();
            if (heroBhv == null)
            {
                throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, "Invalid hero card: {0}", hero.Model.Name));
            }

            Hero = heroBhv;
            m_battlefieldCards.Add(hero);

            Health = heroBhv.Model.Health;
            Mana = 2;
            IsSkillCharged = false;
            ResetManaDelta();
        }

        internal void ResetManaDelta()
        {
            ManaDelta = 2;
        }
    }
}