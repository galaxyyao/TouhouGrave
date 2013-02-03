using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_EnemySummonCostUp :
        BaseBehavior<Passive_EnemySummonCostUp.ModelType>,
        Commands.ICause,
        IPrerequisiteTrigger<Commands.PlayCard>
    {
        public CommandResult RunPrerequisite(Commands.PlayCard command)
        {
            if (command.CardToPlay.Owner != Host.Owner)
            {
                Game.NeedMana(command.CardToPlay.Owner, 1);
            }
            return CommandResult.Pass;
        }

        [BehaviorModel(DefaultName = "The World")]
        public class ModelType : BehaviorModel<Passive_EnemySummonCostUp>
        { }
    }
}
