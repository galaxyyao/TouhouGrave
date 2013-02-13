using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Simulation
{
    class BaseSimulator<TContext>
    {
        public virtual IEnumerable<Choice> TacticalPhase(Interactions.TacticalPhase io, TContext context)
        {
            return Enumerable.Empty<Choice>();
        }

        public virtual IEnumerable<Choice> SelectCards(Interactions.SelectCards io, TContext context)
        {
            return Enumerable.Empty<Choice>();
        }

        public virtual IEnumerable<Choice> MessageBox(Interactions.MessageBox io, TContext context)
        {
            return Enumerable.Empty<Choice>();
        }
    }
}
