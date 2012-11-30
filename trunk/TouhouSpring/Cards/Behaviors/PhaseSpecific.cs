using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TouhouSpring.Commands;

namespace TouhouSpring.Behaviors
{
    public class PhaseSpecific : BaseBehavior<PhaseSpecific.ModelType>,
        IPrerequisiteTrigger<PlayCard>, IPlayable
    {
        CommandResult IPrerequisiteTrigger<PlayCard>.Run(CommandContext<PlayCard> context)
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
