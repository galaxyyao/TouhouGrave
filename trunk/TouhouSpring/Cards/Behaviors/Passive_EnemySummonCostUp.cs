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
        private List<CardModel> costUpModels = new List<CardModel>();

        public void Trigger(Triggers.PostCardPlayedContext context)
        {
            if (context.CardPlayed == Host)
            {
                var hostOpponentPlayer = (context.Game.PlayerPlayer == Host.Owner) ? context.Game.OpponentPlayer : context.Game.PlayerPlayer;
                foreach (var card in hostOpponentPlayer.CardsOnHand)
                {
                    if (card.Behaviors.Get<ManaCost_PrePlay>() == null)
                        throw new MissingMemberException("TouhouSpring.Behaviors.ManaCost_PrePlay Missing for card");
                    if (costUpModels.Contains(card.Model))
                        continue;
                    card.Behaviors.Get<ManaCost_PrePlay>().Model.Cost += 1;
                    costUpModels.Add((CardModel)card.Model);
                    var s = card.Behaviors.Get<ManaCost_PrePlay>();
                }
            }
        }

        public void Trigger(Triggers.CardLeftBattlefieldContext context)
        {
            if (context.CardToLeft == Host)
            {
                var hostOpponentPlayer = (context.Game.PlayerPlayer == Host.Owner) ? context.Game.OpponentPlayer : context.Game.PlayerPlayer;
                foreach (var card in hostOpponentPlayer.CardsOnHand)
                {
                    if (costUpModels.Contains(card.Model))
                    {
                        costUpModels.Remove((CardModel)card.Model);
                        card.Behaviors.Get<ManaCost_PrePlay>().Model.Cost -= 1;
                    }
                }
            }
        }

        public void Trigger(Triggers.PostCardDrawnContext context)
        {
            if (IsOnBattlefield && context.CardDrawn.Owner != Host.Owner)
            {
                if (costUpModels.Contains(context.CardDrawn.Model))
                    return;
                context.CardDrawn.Behaviors.Get<ManaCost_PrePlay>().Model.Cost += 1;
            }
        }

        [BehaviorModel(typeof(Passive_EnemySummonCostUp), DefaultName = "The World")]
        public class ModelType : BehaviorModel
        { }
    }
}
