using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_Inspire : BaseBehavior<Passive_Inspire.ModelType>,
        IGlobalEpilogTrigger<Commands.DealDamageToCard>,
        IGlobalEpilogTrigger<Commands.EndPhase>
    {
        private ValueModifier m_attackModifier;
        private bool m_inspired;

        void IGlobalEpilogTrigger<Commands.DealDamageToCard>.RunGlobalEpilog(Commands.DealDamageToCard command)
        {
            if (!m_inspired && Host.Warrior != null && command.Cause == Host.Warrior)
            {
                foreach (var card in Host.Owner.CardsOnBattlefield)
                {
                    if (card != Host && card.Warrior != null)
                    {
                        Game.QueueCommands(new Commands.SendBehaviorMessage(card.Warrior, WarriorMessage.AddAttackModifier, m_attackModifier));
                    }
                }
                m_inspired = true;
            }
        }

        void IGlobalEpilogTrigger<Commands.EndPhase>.RunGlobalEpilog(Commands.EndPhase command)
        {
            if (m_inspired
                && command.PreviousPhase == "Main"
                && Game.ActingPlayer == Host.Owner)
            {
                foreach (var card in Host.Owner.CardsOnBattlefield)
                {
                    if (card != Host && card.Warrior != null)
                    {
                        Game.QueueCommands(new Commands.SendBehaviorMessage(card.Warrior, WarriorMessage.RemoveAttackModifier, m_attackModifier));
                    }
                }
                m_inspired = false;
            }
        }

        protected override void OnInitialize()
        {
            m_attackModifier = new ValueModifier(ValueModifierOperator.Add, Model.AttackBoost);
            m_inspired = false;
        }

        protected override void OnTransferFrom(IBehavior original)
        {
            m_attackModifier = (original as Passive_Inspire).m_attackModifier;
            m_inspired = (original as Passive_Inspire).m_inspired;
        }

        [BehaviorModel(typeof(Passive_Inspire), Category = "v0.5/Passive", DefaultName = "激励")]
        public class ModelType : BehaviorModel
        {
            public int AttackBoost { get; set; }
        }
    }
}
