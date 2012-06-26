using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public class Passive_EnemySummonCostUp :
        BaseBehavior<Passive_EnemySummonCostUp.ModelType>,
        ITrigger<Triggers.PostCardDrawnContext>,
        ITrigger<Triggers.CardLeftBattlefieldContext>,
        ITrigger<Triggers.PostCardPlayedContext>
    {
        public void Trigger(Triggers.PostCardPlayedContext context)
        {
            if (context.CardPlayed == Host)
            {
                foreach (var card in Host.Owner.CardsOnHand)
                {
                    card.Behaviors.Get<ManaCost_PrePlay>().Model.Cost += 1;
                }
            }
        }

        public void Trigger(Triggers.CardLeftBattlefieldContext context)
        {
            if (context.CardToLeft == Host)
            {
                foreach (var card in Host.Owner.CardsOnHand)
                {
                    card.Behaviors.Get<ManaCost_PrePlay>().Model.Cost -= 1;
                }
            }
        }

        public void Trigger(Triggers.PostCardDrawnContext context)
        {
            if (IsOnBattlefield && context.CardDrawn.Owner != Host.Owner)
            {
                context.CardDrawn.Behaviors.Get<ManaCost_PrePlay>().Model.Cost += 1;
            }
        }

        [BehaviorModel("The World", typeof(Passive_EnemySummonCostUp))]
        public class ModelType : BehaviorModel
        { }
    }
}
