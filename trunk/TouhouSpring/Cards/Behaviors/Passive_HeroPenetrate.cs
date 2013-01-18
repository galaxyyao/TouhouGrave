using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_HeroPenetrate :
        BaseBehavior<Passive_HeroPenetrate.ModelType>,
        Commands.ICause,
        IEpilogTrigger<Commands.DealDamageToCard>
    {
        public void RunEpilog(Commands.DealDamageToCard command)
        {
            if (Host.IsOnBattlefield
                && command.Cause is Warrior
                && (command.Cause as Warrior).Host == Host.Owner.Hero)
            {
                var damagedWarrior = command.Target.Behaviors.Get<Warrior>();
                int overflow = Math.Min(Math.Max(-damagedWarrior.Life, 0), command.DamageToDeal);
                Game.IssueCommands(new Commands.DealDamageToPlayer(command.Target.Owner, overflow, this));
            }
        }

        [BehaviorModel(typeof(Passive_HeroPenetrate), DefaultName = "式神化猫")]
        public class ModelType : BehaviorModel
        { }
    }
}
