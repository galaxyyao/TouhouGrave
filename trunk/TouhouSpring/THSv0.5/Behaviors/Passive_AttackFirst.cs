using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_AttackFirst :
        BaseBehavior<Passive_AttackFirst.ModelType>,
        Commands.ICause,
        IPrologTrigger<Commands.DealDamageToCard>,
        IEpilogTrigger<Commands.DealDamageToCard>
    {
        public void RunProlog(Commands.DealDamageToCard command)
        {
            if (command.Target == Host
                && Host.IsOnBattlefield
                && command.Cause is Warrior)
            {
                var attacker = (Warrior)command.Cause;
                Game.IssueCommands(new Commands.DealDamageToCard(attacker.Host, Host.Behaviors.Get<Warrior>().Attack, this));
            }
        }

        public void RunEpilog(Commands.DealDamageToCard command)
        {
            if (command.Cause != this)
                return;
            var attacker = command.Target.Behaviors.Get<Warrior>();
            if (attacker.Life<=0)
            {
                Game.IssueCommands(new Commands.HealCard(Host, attacker.Attack, this));
            }
        }

        [BehaviorModel(Category = "v0.5/Passive", DefaultName = "先攻")]
        public class ModelType : BehaviorModel<Passive_AttackFirst>
        { }
    }
}

