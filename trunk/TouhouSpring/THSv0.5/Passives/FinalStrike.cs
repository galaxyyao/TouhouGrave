using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TouhouSpring.Behaviors;

namespace TouhouSpring.THSv0_5.Passives
{
    public sealed class FinalStrike : FatalDamage<FinalStrike.ModelType>,
        Commands.ICause
    {
        protected override void OnFatalDamage(IBehavior fatalDamageCause, Warrior hostWarrior)
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

        [BehaviorModel(typeof(FinalStrike), Category = "v0.5/Passive", DefaultName = "自焚")]
        public class ModelType : BehaviorModel
        {
        }
    }
}
