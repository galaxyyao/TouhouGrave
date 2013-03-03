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
                Game.NeedMana(1);
            }
            return CommandResult.Pass;
        }

        [BehaviorModel(typeof(Passive_EnemySummonCostUp), DefaultName = "The World")]
        public class ModelType : BehaviorModel
        { }
    }
}
