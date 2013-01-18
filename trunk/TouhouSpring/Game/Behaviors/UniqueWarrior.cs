using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class UniqueWarrior :
        BaseBehavior<UniqueWarrior.ModelType>,
        Commands.ICause,
        IPrerequisiteTrigger<Commands.PlayCard>
    {
        public CommandResult RunPrerequisite(Commands.PlayCard command)
        {
            if (command.CardToPlay == Host
                && Host.Owner.CardsOnBattlefield.Any(card => card.Model == Host.Model))
                return CommandResult.Cancel();

            return CommandResult.Pass;
        }

        [BehaviorModel(typeof(UniqueWarrior), DefaultName = "唯一")]
        public class ModelType : BehaviorModel
        {
        }
    }
}
