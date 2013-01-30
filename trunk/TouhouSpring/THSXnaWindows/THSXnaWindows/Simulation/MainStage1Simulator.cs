using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Simulation
{
    // in this simulator, mana-consuming actions are done
    class MainStage1Simulator : BaseSimulator
    {
        public override IEnumerable<Choice> TacticalPhase(Interactions.TacticalPhase io, Context context)
        {
            for (int i = 0; i < io.PlayCardCandidates.Count; ++i)
            {
                yield return new PlayCardChoice(i);
            }

            for (int i = 0; i < io.ActivateAssistCandidates.Count; ++i)
            {
                yield return new ActivateAssistChoice(i);
            }

            for (int i = 0; i < io.CastSpellCandidates.Count; ++i)
            {
                yield return new CastSpellChoice(i);
            }

            yield return new PassChoice();
        }
    }
}
