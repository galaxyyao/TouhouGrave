using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_HeroPenetrate :
        BaseBehavior<Passive_HeroPenetrate.ModelType>,
        IEpilogTrigger<Commands.DealDamageToCard>
    {
        void IEpilogTrigger<Commands.DealDamageToCard>.Run(Commands.DealDamageToCard command)
        {
            if (IsOnBattlefield
                && command.Cause is Warrior
                && command.Cause.Host == Host.Owner.Hero.Host)
            {
                var damagedWarrior = command.Target.Behaviors.Get<Warrior>();
                int overflow = Math.Min(Math.Max(damagedWarrior.AccumulatedDamage - damagedWarrior.Defense, 0), command.DamageToDeal);
                command.Game.IssueCommands(new Commands.DealDamageToPlayer(command.Target.Owner, this, overflow));
            }
        }

        [BehaviorModel(typeof(Passive_HeroPenetrate), DefaultName = "式神化猫")]
        public class ModelType : BehaviorModel
        { }
    }
}
