using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TouhouSpring.Commands
{
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
}
