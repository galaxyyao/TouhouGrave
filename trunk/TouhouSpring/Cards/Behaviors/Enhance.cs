using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Enhance : BaseBehavior<Enhance.ModelType>,
        ILocalEpilogTrigger<Commands.AddBehavior>,
        ILocalEpilogTrigger<Commands.RemoveBehavior>
    {
        private ValueModifier m_attackModifier;

        public void RunLocalEpilog(Commands.AddBehavior command)
        {
            if (Host.Warrior != null)
            {
                if (m_attackModifier != null)
                {
                    Game.QueueCommands(new Commands.SendBehaviorMessage(
                        Host.Warrior,
                        "AttackModifiers",
                        new object[] { "add", m_attackModifier }));
                }
            }
            else
            {
                m_attackModifier = null;
            }
        }

        public void RunLocalEpilog(Commands.RemoveBehavior command)
        {
            if (m_attackModifier != null)
            {
                Game.QueueCommands(new Commands.SendBehaviorMessage(
                    Host.Warrior,
                    "AttackModifiers",
                    new object[] { "remove", m_attackModifier }));
                m_attackModifier = null;
            }
        }

        protected override void OnInitialize()
        {
            if (Model.AttackBoost == 0)
            {
                throw new ArgumentException("Attack must not be zero.");
            }

            m_attackModifier = new ValueModifier(ValueModifierOperator.Add, Model.AttackBoost);
        }

        protected override void OnTransferFrom(IBehavior original)
        {
            m_attackModifier = (original as Enhance).m_attackModifier;
        }

        [BehaviorModel(typeof(Enhance), HideFromEditor = true)]
        public class ModelType : BehaviorModel
        {
            public int AttackBoost { get; set; }
        }
    }
}
