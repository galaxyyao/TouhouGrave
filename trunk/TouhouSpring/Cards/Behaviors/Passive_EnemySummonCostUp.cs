using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TouhouSpring.Commands;

namespace TouhouSpring.Behaviors
{
    public class Passive_EnemySummonCostUp :
        BaseBehavior<Passive_EnemySummonCostUp.ModelType>,
        IPrerequisiteTrigger<PlayCard>,
        IPrologTrigger<PlayCard>
    {
        CommandResult IPrerequisiteTrigger<PlayCard>.Run(CommandContext<PlayCard> context)
        {
            if (context.Command.CardToPlay.Owner != Host.Owner)
            {
                if (context.Command.CardToPlay.Owner.FreeMana < 1)
                {
                    return CommandResult.Cancel("Insufficient mana.");
                }
                context.Game.ReserveMana(context.Command.CardToPlay.Owner, 1);
            }
            return CommandResult.Pass;
        }

        void IPrologTrigger<PlayCard>.Run(CommandContext<PlayCard> context)
        {
            if (context.Command.CardToPlay.Owner != Host.Owner)
            {
                context.Game.IssueCommands(new UpdateMana
                {
                    Player = context.Command.CardToPlay.Owner,
                    Amount = -1
                });
            }
        }

        [BehaviorModel(typeof(Passive_EnemySummonCostUp), DefaultName = "The World")]
        public class ModelType : BehaviorModel
        { }
    }
}
