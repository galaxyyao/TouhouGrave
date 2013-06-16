using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TouhouSpring.Commands
{
    public struct Library : IZoneToken
    {
        public int Zone { get { return SystemZone.Library; } }

        public void Add(CardInstance card)
        {
            Debug.Assert(false);
        }

        public void Remove(CardInstance card)
        {
            Debug.Assert(false);
        }

        public bool ValidateAdd(CardInstance card)
        {
            return false;
        }

        public bool ValidateRemove(CardInstance card)
        {
            return false;
        }
    }
}
