using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Simulation
{
    // in this simulator attacks are performed
    class MainStage2Simulator : BaseSimulator
    {
        public override IEnumerable<Choice> TacticalPhase(Interactions.TacticalPhase io, Context context)
        {
            if (io.DefenderCandidates.Count != 0)
            {
                for (int i = 0; i < io.AttackerCandidates.Count; ++i)
                {
                    for (int j = 0; j < io.DefenderCandidates.Count; ++j)
                    {
                        yield return new AttackCardChoice(i, j);
                    }
                }
            }
            else
            {
                for (int i = 0; i < io.AttackerCandidates.Count; ++i)
                {
                    for (int j = 0; j < io.Game.ActingPlayerEnemies.Count(); ++j)
                    {
                        yield return new AttackPlayerChoice(i, j);
                    }
                }
            }

            yield return new PassChoice();
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
