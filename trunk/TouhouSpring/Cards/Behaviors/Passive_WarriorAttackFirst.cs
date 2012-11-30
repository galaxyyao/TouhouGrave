using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TouhouSpring.Commands;

namespace TouhouSpring.Behaviors
{
    public class Passive_WarriorAttackFirst :
        BaseBehavior<Passive_WarriorAttackFirst.ModelType>,
        ITrigger<Triggers.PostCardDamagedContext>,
        IEpilogTrigger<EndTurn>
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

        void IEpilogTrigger<EndTurn>.Run(CommandContext<EndTurn> context)
        {
            if (attackFirstCompensation != null)
            {
                throw new NotImplementedException();
                // TODO: issue commands for the following:
                //Host.Behaviors.Get<Warrior>().Defense.RemoveModifier(attackFirstCompensation);
                //attackFirstCompensation = null;
            }
        }

        [BehaviorModel(typeof(Passive_WarriorAttackFirst), DefaultName = "风神")]
        public class ModelType : BehaviorModel
        { }
    }
}
