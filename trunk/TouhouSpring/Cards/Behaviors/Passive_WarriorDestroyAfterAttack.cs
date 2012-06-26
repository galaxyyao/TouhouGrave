using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public class Passive_WarriorDestroyAfterAttack:
        BaseBehavior<Passive_WarriorDestroyAfterAttack.ModelType>,
        ITrigger<Triggers.PostCardDamagedContext>
    {
        public void Trigger(Triggers.PostCardDamagedContext context)
        {
            if (context.Cause == Host)
            {
                if(context.CardDamaged.Owner.CardsOnBattlefield.Contains(context.CardDamaged))
                    context.Game.DestroyCard(context.CardDamaged);
            }
        }

        [BehaviorModel("死神", typeof(Passive_WarriorDestroyAfterAttack))]
        public class ModelType : BehaviorModel
        { }
    }
}
