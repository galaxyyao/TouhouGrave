using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TouhouSpring.Behaviors;

namespace TouhouSpring.THSv0_5.Passives
{
    public sealed class DeathBomb : FatalDamage<DeathBomb.ModelType>,
        Commands.ICause
    {
        protected override void OnFatalDamage(IBehavior fatalDamageCause, Warrior hostWarrior)
        {
            var warrior = fatalDamageCause as Warrior;
            if (warrior != null)
            {
                Game.QueueCommands(new Commands.DealDamageToCard(warrior.Host, Model.Damage, this));
            }
        }

        [BehaviorModel(typeof(DeathBomb), Category = "v0.5/Passive", DefaultName = "自爆")]
        public class ModelType : BehaviorModel
        {
            public int Damage { get; set; }
        }
    }
}
