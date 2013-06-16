using System;
using System.Collections.Generic;
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
}
