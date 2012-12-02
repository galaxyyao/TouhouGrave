using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TouhouSpring.Commands;

namespace TouhouSpring.Behaviors
{
    public class Passive_CardDrawnNumberUp :
        BaseBehavior<Passive_CardDrawnNumberUp.ModelType>,
        IEpilogTrigger<StartTurn>,
        IEpilogTrigger<DrawCard>
    {
        private bool m_isMoreCardDrawn = false;

        void IEpilogTrigger<StartTurn>.Run(CommandContext<StartTurn> context)
        {
            m_isMoreCardDrawn = false;
        }

        void IEpilogTrigger<DrawCard>.Run(CommandContext<DrawCard> context)
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
                    context.Game.IssueCommands(new DrawCard { PlayerDrawing = Host.Owner });
            }
        }

        [BehaviorModel(typeof(Passive_CardDrawnNumberUp), DefaultName = "星光")]
        public class ModelType : BehaviorModel
        { }
    }
}
