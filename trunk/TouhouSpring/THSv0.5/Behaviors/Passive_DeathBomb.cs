using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_DeathBomb : Passive_OnDeath<Passive_DeathBomb.ModelType>,
        Commands.ICause
    {
        private Warrior m_fatalWarriorCause;

        protected override void OnDeath(IBehavior fatalDamageCause, Warrior hostWarrior)
        {
            var warrior = fatalDamageCause as Warrior;
            if (warrior != null)
            {
                Game.QueueCommands(new Commands.DealDamageToCard(warrior.Host, Model.Damage, this));
            }
        }

        [BehaviorModel(typeof(Passive_DeathBomb), Category = "v0.5/Passive", DefaultName = "死后炸弹")]
        public class ModelType : BehaviorModel
        {
            public int Damage { get; set; }
        }
    }
}
