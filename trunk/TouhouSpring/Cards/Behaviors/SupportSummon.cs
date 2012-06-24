using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public class SupportSummon : BaseBehavior<SupportSummon.ModelType>,
        ITrigger<Triggers.PreCardPlayContext>
    {
        public void Trigger(Triggers.PreCardPlayContext context)
        {
            if (context.CardToPlay == Host)
            {
                //TODO: add logic
                var cardsOnBattlefield = context.Game.PlayerPlayer.CardsOnBattlefield;
                bool hasSupportOnBattlefield = cardsOnBattlefield.Any(card => card.Behaviors.Get<Behaviors.Support>() != null);
                if (hasSupportOnBattlefield)
                {
                    var result = new Interactions.MessageBox(context.Game.PlayerController
                        , "场上已有一张支援卡，要直接从手牌补魔么？"
                        , Interactions.MessageBox.Button.Yes | Interactions.MessageBox.Button.No).Run();
                    if (result == Interactions.MessageBox.Button.Yes)
                    {
                        context.Game.PlayerPlayer.IsSkillCharged = true;
                        context.CardToPlay.Behaviors.Add(new Behaviors.Instant());
                    }
                    else
                    {
                        context.Cancel = true;
                    }
                }
            }
        }

        [BehaviorModel("支援召唤", typeof(SupportSummon))]
        public class ModelType : BehaviorModel
        { }
    }
}
