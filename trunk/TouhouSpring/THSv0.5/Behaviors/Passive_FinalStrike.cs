using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_FinalStrike : Passive_OnDeath<Passive_FinalStrike.ModelType>,
        Commands.ICause
    {
        protected override void OnDeath(IBehavior fatalDamageCause, Warrior hostWarrior)
        {
            var warrior = fatalDamageCause as Warrior;
            if (warrior != null)
            {
                Game.QueueCommands(new Commands.DealDamageToCard(warrior.Host, hostWarrior.Attack, this));
            }
            else
            {
                Game.QueueCommands(new Commands.SubtractPlayerLife(fatalDamageCause.Host.Owner, hostWarrior.Attack, this));
            }
        }

        [BehaviorModel(typeof(Passive_FinalStrike), Category = "v0.5/Passive", DefaultName = "自焚")]
        public class ModelType : BehaviorModel
        {
        }
    }
}
