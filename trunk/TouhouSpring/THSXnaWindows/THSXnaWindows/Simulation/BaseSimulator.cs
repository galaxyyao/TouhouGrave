using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Simulation
{
    class BaseSimulator
    {
        public virtual IEnumerable<Choice> TacticalPhase(Interactions.TacticalPhase io, int highestOrder)
        {
            return Enumerable.Empty<Choice>();
        }

        public virtual IEnumerable<Choice> SelectCards(Interactions.SelectCards io, int highestOrder)
        {
            return Enumerable.Empty<Choice>();
        }

        public virtual IEnumerable<Choice> MessageBox(Interactions.MessageBox io, int highestOrder)
        {
            return Enumerable.Empty<Choice>();
        }
    }
}
