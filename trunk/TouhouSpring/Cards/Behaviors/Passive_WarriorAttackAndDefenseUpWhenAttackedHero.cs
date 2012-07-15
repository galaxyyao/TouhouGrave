using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public class Passive_WarriorAttackAndDefenseUpWhenAttackedHero:
        BaseBehavior<Passive_WarriorAttackAndDefenseUpWhenAttackedHero.ModelType>,
        ITrigger<Triggers.PostPlayerDamagedContext>
    {
        public void Trigger(Triggers.PostPlayerDamagedContext context)
        {
            if (context.Cause == Host)
            {
                var attackMod = new AttackModifier(x => x + 1);
                var defenseMod = new DefenseModifier(y => y + 1);
                Host.Behaviors.Add(attackMod);
                Host.Behaviors.Add(defenseMod);
            }
        }

        [BehaviorModel(typeof(Passive_WarriorAttackAndDefenseUpWhenAttackedHero), DefaultName = "午后红茶")]
        public class ModelType : BehaviorModel
        { }
    }
}
