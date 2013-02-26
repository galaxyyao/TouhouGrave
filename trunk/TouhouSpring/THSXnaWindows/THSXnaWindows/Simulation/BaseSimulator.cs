using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Simulation
{
    class BaseSimulator
    {
        public virtual IEnumerable<Choice> TacticalPhase(Interactions.TacticalPhase io, IContext context)
        {
            return Enumerable.Empty<Choice>();
        }

        public virtual IEnumerable<Choice> SelectCards(Interactions.SelectCards io, IContext context)
        {
            return Enumerable.Empty<Choice>();
        }

        public virtual IEnumerable<Choice> MessageBox(Interactions.MessageBox io, IContext context)
        {
            return Enumerable.Empty<Choice>();
        }

        public virtual IEnumerable<Choice> SelectNumber(Interactions.SelectNumber io, IContext context)
        {
            return Enumerable.Empty<Choice>();
        }
    }
}
