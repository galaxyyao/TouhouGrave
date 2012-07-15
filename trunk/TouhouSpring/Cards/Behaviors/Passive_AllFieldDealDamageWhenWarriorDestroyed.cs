using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public class Passive_AllFieldDealDamageWhenWarriorDestroyed:
        BaseBehavior<Passive_AllFieldDealDamageWhenWarriorDestroyed.ModelType>,
        ITrigger<Triggers.CardEnteredGraveyardContext>
    {
        public void Trigger(Triggers.CardEnteredGraveyardContext context)
        {
            if (context.Card == Host)
            {
                foreach (BaseCard card in context.Game.PlayerPlayer.CardsOnBattlefield)
                {
                    if (card.Behaviors.Get<Hero>() != null)
                        continue;
                    var warrior = card.Behaviors.Get<Warrior>();
                    if(warrior!=null)
                        warrior.AccumulatedDamage += Model.Damage;
                }
                foreach (BaseCard card in context.Game.OpponentPlayer.CardsOnBattlefield)
                {
                    if (card.Behaviors.Get<Hero>() != null)
                        continue;
                    var warrior = card.Behaviors.Get<Warrior>();
                    if (warrior != null)
                        warrior.AccumulatedDamage += Model.Damage;
                }
            }
        }

        [BehaviorModel(typeof(Passive_AllFieldDealDamageWhenWarriorDestroyed), DefaultName = "凤凰")]
        public class ModelType : BehaviorModel
        {
            public int Damage { get; set; }
        }
    }
}
