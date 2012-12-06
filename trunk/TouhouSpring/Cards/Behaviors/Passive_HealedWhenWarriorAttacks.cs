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
        void IEpilogTrigger<Commands.DealDamageToCard>.Run(CommandContext<Commands.DealDamageToCard> context)
        {
            if (context.Command.Target.Owner != Host.Owner
                && IsOnBattlefield
                && context.Command.Cause is Warrior
                && context.Command.Cause.Host.Owner == Host.Owner
                && !context.Command.Cause.Host.Behaviors.Has<Behaviors.Hero>())
            {
                context.Game.IssueCommands(new Commands.HealPlayer
                {
                    Player = Host.Owner,
                    Amount = context.Command.DamageToDeal,
                    Cause = this
                });
            }
        }

        [BehaviorModel(typeof(Passive_HealedWhenWarriorAttacks), DefaultName = "吸血鬼")]
        public class ModelType : BehaviorModel
        { }
    }
}
