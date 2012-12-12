using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_CardDrawnNumberUp :
        BaseBehavior<Passive_CardDrawnNumberUp.ModelType>,
        IEpilogTrigger<Commands.StartTurn>,
        IEpilogTrigger<Commands.DrawCard>
    {
        private bool m_isMoreCardDrawn = false;

        void IEpilogTrigger<Commands.StartTurn>.Run(Commands.StartTurn command)
        {
            m_isMoreCardDrawn = false;
        }

        void IEpilogTrigger<Commands.DrawCard>.Run(Commands.DrawCard command)
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
                    command.Game.IssueCommands(new Commands.DrawCard(Host.Owner));
            }
        }

        [BehaviorModel(typeof(Passive_CardDrawnNumberUp), DefaultName = "星光")]
        public class ModelType : BehaviorModel
        { }
    }
}
