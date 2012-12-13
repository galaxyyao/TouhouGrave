using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_HealedWhenWarriorAttacks :
        BaseBehavior<Passive_HealedWhenWarriorAttacks.ModelType>,
        Commands.ICause,
        IEpilogTrigger<Commands.DealDamageToCard>
    {
        void IEpilogTrigger<Commands.DealDamageToCard>.Run(Commands.DealDamageToCard command)
        {
            if (command.Target.Owner != Host.Owner
                && IsOnBattlefield
                && command.Cause is Warrior
                && (command.Cause as Warrior).Host.Owner == Host.Owner
                && !(command.Cause as Warrior).Host.Behaviors.Has<Behaviors.Hero>())
            {
                Game.IssueCommands(new Commands.HealPlayer(Host.Owner, command.DamageToDeal, this));
            }
        }

        [BehaviorModel(typeof(Passive_HealedWhenWarriorAttacks), DefaultName = "吸血鬼")]
        public class ModelType : BehaviorModel
        { }
    }
}
