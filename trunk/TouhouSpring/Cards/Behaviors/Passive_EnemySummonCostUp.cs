using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public class Passive_EnemySummonCostUp :
        BaseBehavior<Passive_EnemySummonCostUp.ModelType>,
        IPrerequisiteTrigger<Commands.PlayCard>,
        IPrologTrigger<Commands.PlayCard>
    {
        CommandResult IPrerequisiteTrigger<Commands.PlayCard>.Run(Commands.PlayCard command)
        {
            if (command.CardToPlay.Owner != Host.Owner)
            {
                if (command.CardToPlay.Owner.FreeMana < 1)
                {
                    return CommandResult.Cancel("Insufficient mana.");
                }
                command.Game.ReserveMana(command.CardToPlay.Owner, 1);
            }
            return CommandResult.Pass;
        }

        void IPrologTrigger<Commands.PlayCard>.Run(Commands.PlayCard command)
        {
            if (command.CardToPlay.Owner != Host.Owner)
            {
                command.Game.IssueCommands(new Commands.UpdateMana(command.CardToPlay.Owner, -1));
            }
        }

        [BehaviorModel(typeof(Passive_EnemySummonCostUp), DefaultName = "The World")]
        public class ModelType : BehaviorModel
        { }
    }
}
