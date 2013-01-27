using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Simulation
{
    class PlayCardSimulator : BaseSimulator
    {
        public override IEnumerable<Choice> TacticalPhase(Interactions.TacticalPhase io, Context context)
        {
            for (int i = 0; i < io.PlayCardCandidates.Count; ++i)
            {
                yield return new PlayCardChoice(i);
            }

            yield return new PassChoice();
        }
    }
}
