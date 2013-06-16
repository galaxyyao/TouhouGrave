using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TouhouSpring.Commands
{
    public interface IZoneToken
    {
        int Zone { get; }
        void Add(CardInstance card);
        void Remove(CardInstance card);
        bool ValidateAdd(CardInstance card);
        bool ValidateRemove(CardInstance card);
    }

    public struct Battlefield : IZoneToken
    {
        public int Zone { get { return SystemZone.Battlefield; } }

        public void Add(CardInstance card)
        {
            Debug.Assert(ValidateAdd(card));
            card.Owner.m_battlefieldCards.Add(card);
            card.Owner.Game.SubscribeCardToCommands(card);
        }

        public void Remove(CardInstance card)
        {
            Debug.Assert(ValidateRemove(card));
            card.Owner.m_battlefieldCards.Remove(card);
            card.Owner.Game.UnsubscribeCardFromCommands(card);
        }

        public bool ValidateAdd(CardInstance card)
        {
            return !card.Owner.m_battlefieldCards.Contains(card);
        }

        public bool ValidateRemove(CardInstance card)
        {
            return card.Owner.m_battlefieldCards.Contains(card);
        }
    }

    public struct Assist : IZoneToken
    {
        public int Zone { get { return SystemZone.Assist; } }

        public void Add(CardInstance card)
        {
            Debug.Assert(ValidateAdd(card));
            card.Owner.m_assists.Add(card);
        }

        public void Remove(CardInstance card)
        {
            Debug.Assert(ValidateRemove(card));
            card.Owner.m_assists.Remove(card);
        }

        public bool ValidateAdd(CardInstance card)
        {
            return !card.Owner.m_assists.Contains(card);
        }

        public bool ValidateRemove(CardInstance card)
        {
            return card.Owner.m_assists.Contains(card);
        }
    }

    public struct Hand : IZoneToken
    {
        public int Zone { get { return SystemZone.Hand; } }

        public void Add(CardInstance card)
        {
            Debug.Assert(ValidateAdd(card));
            card.Owner.AddToHandSorted(card);
        }

        public void Remove(CardInstance card)
        {
            Debug.Assert(ValidateRemove(card));
            card.Owner.m_handSet.Remove(card);
        }

        public bool ValidateAdd(CardInstance card)
        {
            return !card.Owner.m_handSet.Contains(card);
        }

        public bool ValidateRemove(CardInstance card)
        {
            return card.Owner.m_handSet.Contains(card);
        }
    }

    public struct Sacrifice : IZoneToken
    {
        public int Zone { get { return SystemZone.Sacrifice; } }

        public void Add(CardInstance card)
        {
            Debug.Assert(ValidateAdd(card));
            card.Owner.AddToSacrificeSorted(card);
        }

        public void Remove(CardInstance card)
        {
            Debug.Assert(ValidateRemove(card));
            card.Owner.m_sacrifices.Remove(card);
        }

        public bool ValidateAdd(CardInstance card)
        {
            return !card.Owner.m_sacrifices.Contains(card);
        }

        public bool ValidateRemove(CardInstance card)
        {
            return card.Owner.m_sacrifices.Contains(card);
        }
    }
}
