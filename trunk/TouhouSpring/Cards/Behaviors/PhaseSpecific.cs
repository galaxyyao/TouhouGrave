using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public class PhaseSpecific : BaseBehavior<PhaseSpecific.ModelType>,
        Commands.IPrerequisiteTrigger<Commands.PlayCard>, IPlayable
    {
        Commands.Result Commands.IPrerequisiteTrigger<Commands.PlayCard>.Run(Commands.CommandContext context)
        {
            var command = context.Command as Commands.PlayCard;
            if (command.CardToPlay == Host && !IsPlayable(context.Game))
            {
                return Commands.Result.Cancel(String.Format("{0} can't be played in {1} phase.", Host.Model.Name, context.Game.CurrentPhase));
            }

            return Commands.Result.Pass;
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
