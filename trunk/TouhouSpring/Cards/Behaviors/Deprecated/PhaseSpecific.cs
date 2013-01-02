using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class PhaseSpecific : BaseBehavior<PhaseSpecific.ModelType>,
        IPrerequisiteTrigger<Commands.PlayCard>
    {
        CommandResult IPrerequisiteTrigger<Commands.PlayCard>.Run(Commands.PlayCard command)
        {
            if (command.CardToPlay == Host && !IsPlayable())
            {
                return CommandResult.Cancel(String.Format("{0} can't be played in {1} phase.", Host.Model.Name, Game.CurrentPhase));
            }

            return CommandResult.Pass;
        }

        private bool IsPlayable()
        {
            return Model.TacticalPhase && Game.CurrentPhase == "Tactical" && Game.ActingPlayer == Host.Owner
                   || Model.BlockPhase && Game.CurrentPhase == "Combat/Block" && Game.ActingPlayer != Host.Owner;
        }

        [BehaviorModel(typeof(PhaseSpecific))]
        public class ModelType : BehaviorModel
        {
            public bool TacticalPhase { get; set; }
            public bool BlockPhase { get; set; }

            public ModelType()
            {
                TacticalPhase = true;
            }
        }
    }
}
