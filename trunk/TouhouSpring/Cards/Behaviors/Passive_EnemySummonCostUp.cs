using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TouhouSpring.Commands;

namespace TouhouSpring.Behaviors
{
    public class Passive_EnemySummonCostUp :
        BaseBehavior<Passive_EnemySummonCostUp.ModelType>,
        IEpilogTrigger<DrawCard>,
        ITrigger<Triggers.CardLeftBattlefieldContext>,
        IEpilogTrigger<PlayCard>
    {
        private List<CardModel> costUpModels = new List<CardModel>();

        void IEpilogTrigger<PlayCard>.Run(CommandContext<PlayCard> context)
        {
            if (context.Command.CardToPlay == Host)
            {
                var hostOpponentPlayer = (context.Game.PlayerPlayer == Host.Owner) ? context.Game.OpponentPlayer : context.Game.PlayerPlayer;
                foreach (var card in hostOpponentPlayer.CardsOnHand)
                {
                    if (card.Behaviors.Get<ManaCost_PrePlay>() == null)
                        throw new MissingMemberException("TouhouSpring.Behaviors.ManaCost_PrePlay Missing for card");
                    if (costUpModels.Contains(card.Model))
                        continue;

                    throw new NotImplementedException();
                    // TODO: issue commands for the following:
                    //card.Behaviors.Get<ManaCost_PrePlay>().Model.Cost += 1;
                    //costUpModels.Add((CardModel)card.Model);
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

        void IEpilogTrigger<DrawCard>.Run(CommandContext<DrawCard> context)
        {
            if (IsOnBattlefield && context.Command.CardDrawn.Owner != Host.Owner)
            {
                if (costUpModels.Contains(context.Command.CardDrawn.Model))
                    return;

                throw new NotImplementedException();
                // TODO: issue command for the following:
                //context.CardDrawn.Behaviors.Get<ManaCost_PrePlay>().Model.Cost += 1;
            }
        }

        [BehaviorModel(typeof(Passive_EnemySummonCostUp), DefaultName = "The World")]
        public class ModelType : BehaviorModel
        { }
    }
}
