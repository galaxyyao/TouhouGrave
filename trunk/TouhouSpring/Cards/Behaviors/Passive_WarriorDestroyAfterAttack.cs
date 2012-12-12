using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_WarriorDestroyAfterAttack :
        BaseBehavior<Passive_WarriorDestroyAfterAttack.ModelType>,
        IEpilogTrigger<Commands.DealDamageToCard>
    {
        void IEpilogTrigger<Commands.DealDamageToCard>.Run(Commands.DealDamageToCard command)
        {
            if (command.Cause != null
                && command.Cause.Host == Host)
            {
                command.Game.IssueCommands(new Commands.Kill(command.Target, this));
            }
        }

        [BehaviorModel(typeof(Passive_WarriorDestroyAfterAttack), DefaultName = "死神")]
        public class ModelType : BehaviorModel
        { }
    }
}
