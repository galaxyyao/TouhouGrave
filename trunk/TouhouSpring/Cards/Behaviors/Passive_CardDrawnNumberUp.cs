using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public class Passive_CardDrawnNumberUp :
        BaseBehavior<Passive_CardDrawnNumberUp.ModelType>,
        ITrigger<Triggers.PostCardDrawnContext>,
        ITrigger<Triggers.PlayerTurnStartedContext>
    {
        private bool m_isMoreCardDrawn = false;

        public void Trigger(Triggers.PostCardDrawnContext context)
        {
            if (IsOnBattlefield && !m_isMoreCardDrawn)
            {
                m_isMoreCardDrawn = true;
                int hostCardNumber = 0;
                foreach (var card in Host.Owner.CardsOnBattlefield)
                {
                    if (card.Behaviors.Get<Passive_CardDrawnNumberUp>() != null)
                        hostCardNumber++;
                }
                for (int i = 0; i < hostCardNumber; i++)
                    context.Game.DrawCard(Host.Owner);
            }
        }

        public void Trigger(Triggers.PlayerTurnStartedContext context)
        {
            m_isMoreCardDrawn = false;
        }

        [BehaviorModel(typeof(Passive_CardDrawnNumberUp), DefaultName = "星光")]
        public class ModelType : BehaviorModel
        { }
    }
}
