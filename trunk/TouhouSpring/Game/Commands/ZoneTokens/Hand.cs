using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TouhouSpring.Commands
{
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
}
