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
    public sealed partial class Player
    {
        // TODO: command for manipulating these lists
        private List<ValueModifier> m_manaAddModifiers = new List<ValueModifier>();
        private List<ValueModifier> m_manaSubtractModifiers = new List<ValueModifier>();
        private List<ValueModifier> m_lifeAddModifiers = new List<ValueModifier>();
        private List<ValueModifier> m_lifeSubtractModifiers = new List<ValueModifier>();

        internal List<CardInstance> m_handSet = new List<CardInstance>();
        internal List<CardInstance> m_sacrifices = new List<CardInstance>();
        internal List<CardInstance> m_battlefieldCards = new List<CardInstance>();
        internal List<CardInstance> m_assists = new List<CardInstance>();
        internal List<CardInstance> m_activatedAssists = new List<CardInstance>();
        internal List<ICardModel> m_library = new List<ICardModel>();
        internal List<ICardModel> m_graveyard = new List<ICardModel>();

        /// <summary>
        /// Return a collection of cards on hand.
        /// </summary>
        public IIndexable<CardInstance> CardsOnHand
        {
            get; private set;
        }

        public IIndexable<CardInstance> CardsSacrificed
        {
            get; private set;
        }

        /// <summary>
        /// Return a collection of cards on battlefield.
        /// </summary>
        public IIndexable<CardInstance> CardsOnBattlefield
        {
            get; private set;
        }

        // summoned hero is in CardsOnBattlefield collection
        public CardInstance Hero
        {
            get; private set;
        }

        public IIndexable<CardInstance> Assists
        {
            get; private set;
        }

        public IIndexable<CardInstance> ActivatedAssits
        {
            get; private set;
        }

        public Pile Library
        {
            get; private set;
        }

        public Pile Graveyard
        {
            get; private set;
        }

        public string Name
        {
            get; private set;
        }

        public int Health
        {
            get; internal set;
        }

        public int Mana
        {
            get; internal set;
        }

        public int MaxMana
        {
            get { return CardsSacrificed.Count; }
        }

        public int Index
        {
            get; private set;
        }

        public Game Game
        {
            get; private set;
        }

        public int CalculateFinalManaAdd(int amount)
        {
            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException("amount", "Amount must be greater than or equal to zero.");
            }
            return m_manaAddModifiers.Aggregate(amount, (v, mod) => mod.Process(v));
        }

        public int CalculateFinalManaSubtract(int amount)
        {
            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException("amount", "Amount must be greater than or equal to zero.");
            }
            return m_manaSubtractModifiers.Aggregate(amount, (v, mod) => mod.Process(v));
        }

        public int CalculateFinalLifeAdd(int amount)
        {
            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException("amount", "Amount must be greater than or equal to zero.");
            }
            return m_lifeAddModifiers.Aggregate(amount, (v, mod) => mod.Process(v));
        }

        public int CalculateFinalLifeSubtract(int amount)
        {
            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException("amount", "Amount must be greater than or equal to zero.");
            }
            return m_lifeSubtractModifiers.Aggregate(amount, (v, mod) => mod.Process(v));
        }

        internal Player(string name, int playerIndex, Game game)
        {
            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }
            else if (game == null)
            {
                throw new ArgumentNullException("game");
            }
            else if (playerIndex < 0)
            {
                throw new ArgumentOutOfRangeException("playerIndex", "PlayerIndex shall be greater than or equal to zero.");
            }

            CardsOnHand = m_handSet.ToIndexable();
            CardsSacrificed = m_sacrifices.ToIndexable();
            CardsOnBattlefield = m_battlefieldCards.ToIndexable();
            Assists = m_assists.ToIndexable();
            ActivatedAssits = m_activatedAssists.ToIndexable();
            Library = new Pile(m_library);
            Graveyard = new Pile(m_graveyard);

            Name = name;
            Game = game;
            Index = playerIndex;
        }

        /// <summary>
        /// Initialize a player's states.
        /// </summary>
        /// <param name="deck">The deck from which a library pile is generated.</param>
        internal void Initialize(Deck deck)
        {
            if (deck == null)
            {
                throw new ArgumentNullException("deck");
            }

            // initialize player's library
            foreach (var cardModel in deck)
            {
                Library.AddToTop(cardModel);
            }

            // initialize player's hero
            //Hero = new CardInstance(deck.Hero, this);

            // initialize assists
            foreach (var cardModel in deck.Assists)
            {
                m_assists.Add(new CardInstance(cardModel, this));
            }

            Health = 20;
            Mana = 0;
        }

        private class HandSorter : IComparer<CardInstance>
        {
            public int Compare(CardInstance card1, CardInstance card2)
            {
                // Warrior, Spell
                var isWarrior1 = card1.Behaviors.Has<Behaviors.Warrior>();
                var isWarrior2 = card2.Behaviors.Has<Behaviors.Warrior>();
                if (isWarrior1 != isWarrior2)
                {
                    return isWarrior1 ? -1 : 1;
                }

                var isSpell1 = card1.Behaviors.Has<Behaviors.Instant>();
                var isSpell2 = card2.Behaviors.Has<Behaviors.Instant>();
                if (isSpell1 != isSpell2)
                {
                    return isSpell1 ? -1 : 1;
                }

                // The comparison must ensure culture neutral-ness
                return String.Compare(card1.Model.Name, card2.Model.Name, false, CultureInfo.InvariantCulture);
            }
        }

        internal void AddToHandSorted(CardInstance card)
        {
            AddToListSorted(m_handSet, card);
        }

        internal void AddToSacrificeSorted(CardInstance card)
        {
            AddToListSorted(m_sacrifices, card);
        }

        private static void AddToListSorted(List<CardInstance> list, CardInstance card)
        {
            var comparer = new HandSorter();
            var index = list.BinarySearch(card, comparer);
            if (index >= 0)
            {
                for (int i = index + 1; i < list.Count; ++i)
                {
                    if (comparer.Compare(list[i], card) == 1)
                    {
                        list.Insert(i, card);
                        return;
                    }
                }
                list.Add(card);
            }
            else
            {
                list.Insert(~index, card);
            }
        }
    }
}