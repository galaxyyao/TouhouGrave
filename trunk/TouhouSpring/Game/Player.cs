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
    public sealed class Player
    {
        private Profile m_profile;

        internal List<BaseCard> m_handSet = new List<BaseCard>();
        internal List<BaseCard> m_sacrifices = new List<BaseCard>();
        internal List<BaseCard> m_battlefieldCards = new List<BaseCard>();
        internal Pile m_library = new Pile();
        internal Pile m_graveyard = new Pile();

        /// <summary>
        /// Return a collection of cards on hand.
        /// </summary>
        public IIndexable<BaseCard> CardsOnHand
        {
            get; private set;
        }

        public IIndexable<BaseCard> CardsSacrificed
        {
            get; private set;
        }

        /// <summary>
        /// Return a collection of cards on battlefield.
        /// </summary>
        public IIndexable<BaseCard> CardsOnBattlefield
        {
            get; private set;
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

        public int MaxMana
        {
            get { return CardsSacrificed.Count; }
        }

        public int ReservedMana
        {
            get; internal set;
        }

        public int FreeMana
        {
            get { return Mana - ReservedMana; }
        }

        public bool IsSkillCharged
        {
            get; internal set;
        }

        public Behaviors.Hero Hero
        {
            get; private set;
        }

        public Game Game
        {
            get; private set;
        }

        public BaseController Controller
        {
            get; private set;
        }

        internal Player(Profile profile, Game game, BaseController controller)
        {
            if (profile == null)
            {
                throw new ArgumentNullException("profile");
            }
            else if (game == null)
            {
                throw new ArgumentNullException("game");
            }
            else if (controller == null)
            {
                throw new ArgumentNullException("controller");
            }
            else if (controller.Player != null)
            {
                throw new InvalidOperationException("The controller is bound to some player before.");
            }

            CardsOnHand = m_handSet.ToIndexable();
            CardsSacrificed = m_sacrifices.ToIndexable();
            CardsOnBattlefield = m_battlefieldCards.ToIndexable();

            m_profile = profile;
            Game = game;
            Controller = controller;
            Controller.Player = this;
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

            Health = heroBhv.InitialHealth;
            IsSkillCharged = false;
        }
    }
}