using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public class Passive_HeroPenetrate :
        BaseBehavior<Passive_HeroPenetrate.ModelType>,
        IEpilogTrigger<Commands.DealDamageToCard>
    {
        void IEpilogTrigger<Commands.DealDamageToCard>.Run(CommandContext<Commands.DealDamageToCard> context)
        {
            if (IsOnBattlefield
                && context.Command.Cause is Warrior
                && context.Command.Cause.Host == Host.Owner.Hero.Host)
            {
                var damagedWarrior = context.Command.Target.Behaviors.Get<Warrior>();
                int overflow = Math.Min(Math.Max(damagedWarrior.AccumulatedDamage - damagedWarrior.Defense, 0), context.Command.DamageToDeal);
                context.Game.IssueCommands(new Commands.DealDamageToPlayer
                {
                    Player = context.Command.Target.Owner,
                    DamageToDeal = overflow,
                    Cause = this
                });
            }
        }

        [BehaviorModel(typeof(Passive_HeroPenetrate), DefaultName = "式神化猫")]
        public class ModelType : BehaviorModel
        { }
    }
}
