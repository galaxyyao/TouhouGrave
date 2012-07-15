using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public class Passive_WarriorAttackFirst :
        BaseBehavior<Passive_WarriorAttackFirst.ModelType>,
        ITrigger<Triggers.PostCardDamagedContext>,
        ITrigger<Triggers.PlayerTurnEndedContext>
    {
        private Func<int, int> attackFirstCompensation = null;

        public void Trigger(Triggers.PostCardDamagedContext context)
        {
            if (context.Cause == Host.Behaviors.Get<Warrior>())
            {
                var warriorAttackedBhv=context.CardDamaged.Behaviors.Get<Warrior>();
                if (warriorAttackedBhv.AccumulatedDamage >= warriorAttackedBhv.Defense)
                {
                    int damageWontDeal = context.CardDamaged.Behaviors.Get<Warrior>().Attack;
                    attackFirstCompensation = x => x + damageWontDeal;
                    Host.Behaviors.Get<Warrior>().Defense.AddModifierToTail(attackFirstCompensation);
                }
            }
        }

        public void Trigger(Triggers.PlayerTurnEndedContext context)
        {
            if (attackFirstCompensation != null)
            {
                Host.Behaviors.Get<Warrior>().Defense.RemoveModifier(attackFirstCompensation);
                attackFirstCompensation = null;
            }
        }

        [BehaviorModel(typeof(Passive_WarriorAttackFirst), DefaultName = "风神")]
        public class ModelType : BehaviorModel
        { }
    }
}
