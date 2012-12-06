using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public class PhaseSpecific : BaseBehavior<PhaseSpecific.ModelType>,
        IPrerequisiteTrigger<Commands.PlayCard>, IPlayable
    {
        CommandResult IPrerequisiteTrigger<Commands.PlayCard>.Run(CommandContext<Commands.PlayCard> context)
        {
            if (context.Command.CardToPlay == Host && !IsPlayable(context.Game))
            {
                return CommandResult.Cancel(String.Format("{0} can't be played in {1} phase.", Host.Model.Name, context.Game.CurrentPhase));
            }

            return CommandResult.Pass;
        }

        public bool IsPlayable(Game game)
        {
            return Model.TacticalPhase && game.CurrentPhase == "Tactical" && game.PlayerPlayer == Host.Owner
                   || Model.BlockPhase && game.CurrentPhase == "Combat/Block" && game.OpponentPlayer == Host.Owner;
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
