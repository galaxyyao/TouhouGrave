using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public class Passive_CardDrawnNumberUp :
        BaseBehavior<Passive_CardDrawnNumberUp.ModelType>,
        IEpilogTrigger<Commands.StartTurn>,
        IEpilogTrigger<Commands.DrawCard>
    {
        private bool m_isMoreCardDrawn = false;

        void IEpilogTrigger<Commands.StartTurn>.Run(CommandContext<Commands.StartTurn> context)
        {
            m_isMoreCardDrawn = false;
        }

        void IEpilogTrigger<Commands.DrawCard>.Run(CommandContext<Commands.DrawCard> context)
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
                    context.Game.IssueCommands(new Commands.DrawCard { PlayerDrawing = Host.Owner });
            }
        }

        [BehaviorModel(typeof(Passive_CardDrawnNumberUp), DefaultName = "星光")]
        public class ModelType : BehaviorModel
        { }
    }
}
