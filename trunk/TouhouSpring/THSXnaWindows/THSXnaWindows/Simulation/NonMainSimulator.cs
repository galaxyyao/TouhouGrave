using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Simulation
{
    class NonMainSimulator : BaseSimulator
    {
        public override IEnumerable<Choice> TacticalPhase(Interactions.TacticalPhase io, Context context)
        {
            throw new InvalidOperationException("NonMainSimulator doesn't make choice for tactical phase.");
        }

        public override IEnumerable<Choice> SelectCards(Interactions.SelectCards io, Context context)
        {
            for (int i = 0; i < io.SelectFromSet.Count; ++i)
            {
                yield return new SelectCardChoice(i);
            }
        }
    }
}
