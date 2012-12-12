using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public class Enhance : SimpleBehavior<Enhance>,
        IEpilogTrigger<Commands.AddBehavior>,
        IPrologTrigger<Commands.RemoveBehavior>
    {
        private Warrior.ValueModifier m_attackModifier;
        private Warrior.ValueModifier m_defenseModifier;

        public Enhance(int attackMod, int defenseMod)
        {
            if (attackMod == 0 && defenseMod == 0)
            {
                throw new ArgumentException("Attack and defense must not both be zero.");
            }

            m_attackModifier = attackMod != 0 ? new Warrior.ValueModifier(Warrior.ValueModifierOperator.Add, attackMod) : null;
            m_defenseModifier = defenseMod != 0 ? new Warrior.ValueModifier(Warrior.ValueModifierOperator.Add, defenseMod) : null;
        }

        void IEpilogTrigger<Commands.AddBehavior>.Run(Commands.AddBehavior command)
        {
            if (command.Behavior == this)
            {
                if (command.Target.Behaviors.Has<Warrior>())
                {
                    if (m_attackModifier != null)
                    {
                        command.Game.IssueCommands(new Commands.SendBehaviorMessage(
                            command.Target.Behaviors.Get<Warrior>(),
                            "AttackModifiers",
                            new object[] { "add", m_attackModifier }));
                    }
                    if (m_defenseModifier != null)
                    {
                        command.Game.IssueCommands(new Commands.SendBehaviorMessage(
                            command.Target.Behaviors.Get<Warrior>(),
                            "DefenseModifiers",
                            new object[] { "add", m_defenseModifier }));
                    }
                }
                else
                {
                    m_attackModifier = null;
                    m_defenseModifier = null;
                }
            }
        }

        void IPrologTrigger<Commands.RemoveBehavior>.Run(Commands.RemoveBehavior command)
        {
            if (command.Behavior == this)
            {
                if (m_attackModifier != null)
                {
                    command.Game.IssueCommands(new Commands.SendBehaviorMessage(
                        Host.Behaviors.Get<Warrior>(),
                        "AttackModifiers",
                        new object[] { "remove", m_attackModifier }));
                    m_attackModifier = null;
                }
                if (m_defenseModifier != null)
                {
                    command.Game.IssueCommands(new Commands.SendBehaviorMessage(
                        Host.Behaviors.Get<Warrior>(),
                        "DefenseModifiers",
                        new object[] { "remove", m_defenseModifier }));
                    m_defenseModifier = null;
                }
            }
        }
    }
}
