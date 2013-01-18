using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_EnemySummonCostUp :
        BaseBehavior<Passive_EnemySummonCostUp.ModelType>,
        Commands.ICause,
        IPrerequisiteTrigger<Commands.PlayCard>,
        IPrologTrigger<Commands.PlayCard>
    {
        public CommandResult RunPrerequisite(Commands.PlayCard command)
        {
            if (command.CardToPlay.Owner != Host.Owner)
            {
                if (command.CardToPlay.Owner.FreeMana < 1)
                {
                    return CommandResult.Cancel("Insufficient mana.");
                }
                Game.ReserveMana(command.CardToPlay.Owner, 1);
            }
            return CommandResult.Pass;
        }

        public void RunProlog(Commands.PlayCard command)
        {
            if (command.CardToPlay.Owner != Host.Owner)
            {
                Game.IssueCommands(new Commands.UpdateMana(command.CardToPlay.Owner, -1, this));
            }
        }

        [BehaviorModel(typeof(Passive_EnemySummonCostUp), DefaultName = "The World")]
        public class ModelType : BehaviorModel
        { }
    }
}
