using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TouhouSpring.Commands
{
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
