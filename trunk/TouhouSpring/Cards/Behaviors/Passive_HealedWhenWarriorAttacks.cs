using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public class Passive_HealedWhenWarriorAttacks :
        BaseBehavior<Passive_HealedWhenWarriorAttacks.ModelType>,
        IEpilogTrigger<Commands.DealDamageToCard>
    {
        void IEpilogTrigger<Commands.DealDamageToCard>.Run(Commands.DealDamageToCard command)
        {
            if (command.Target.Owner != Host.Owner
                && IsOnBattlefield
                && command.Cause is Warrior
                && command.Cause.Host.Owner == Host.Owner
                && !command.Cause.Host.Behaviors.Has<Behaviors.Hero>())
            {
                command.Game.IssueCommands(new Commands.HealPlayer(Host.Owner, this, command.DamageToDeal));
            }
        }

        [BehaviorModel(typeof(Passive_HealedWhenWarriorAttacks), DefaultName = "吸血鬼")]
        public class ModelType : BehaviorModel
        { }
    }
}
