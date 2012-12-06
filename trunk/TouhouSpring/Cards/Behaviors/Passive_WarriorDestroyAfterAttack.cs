using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public class Passive_WarriorDestroyAfterAttack:
        BaseBehavior<Passive_WarriorDestroyAfterAttack.ModelType>,
        IEpilogTrigger<Commands.DealDamageToCard>
    {
        void IEpilogTrigger<Commands.DealDamageToCard>.Run(CommandContext<Commands.DealDamageToCard> context)
        {
            if (context.Command.Cause != null
                && context.Command.Cause.Host == Host)
            {
                context.Game.IssueCommands(new Commands.Kill
                {
                    Target = context.Command.Target,
                    Cause = this
                });
            }
        }

        [BehaviorModel(typeof(Passive_WarriorDestroyAfterAttack), DefaultName = "死神")]
        public class ModelType : BehaviorModel
        { }
    }
}
