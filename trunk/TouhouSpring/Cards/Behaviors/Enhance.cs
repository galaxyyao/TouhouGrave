using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Enhance : BaseBehavior<Enhance.ModelType>,
        IEpilogTrigger<Commands.AddBehavior>,
        IPrologTrigger<Commands.RemoveBehavior>
    {
        private Warrior.ValueModifier m_attackModifier;

        public void RunEpilog(Commands.AddBehavior command)
        {
            if (command.Behavior == this)
            {
                if (command.Target.Behaviors.Has<Warrior>())
                {
                    if (m_attackModifier != null)
                    {
                        Game.IssueCommands(new Commands.SendBehaviorMessage(
                            command.Target.Behaviors.Get<Warrior>(),
                            "AttackModifiers",
                            new object[] { "add", m_attackModifier }));
                    }
                }
                else
                {
                    m_attackModifier = null;
                }
            }
        }

        public void RunProlog(Commands.RemoveBehavior command)
        {
            if (command.Behavior == this)
            {
                if (m_attackModifier != null)
                {
                    Game.IssueCommands(new Commands.SendBehaviorMessage(
                        Host.Behaviors.Get<Warrior>(),
                        "AttackModifiers",
                        new object[] { "remove", m_attackModifier }));
                    m_attackModifier = null;
                }
            }
        }

        protected override void OnInitialize()
        {
            if (Model.AttackBoost == 0)
            {
                throw new ArgumentException("Attack must not be zero.");
            }

            m_attackModifier = new Warrior.ValueModifier(Warrior.ValueModifierOperator.Add, Model.AttackBoost);
        }

        public class ModelType : BehaviorModel
        {
            public int AttackBoost { get; set; }
        }
    }
}
