﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TouhouSpring.Commands;

namespace TouhouSpring.Behaviors
{
    public class Enhance : SimpleBehavior<Enhance>,
        IEpilogTrigger<AddBehavior>,
        IPrologTrigger<RemoveBehavior>
    {
        private Warrior.ValueModifier m_attackModifier;
        private Warrior.ValueModifier m_defenseModifier;

        public Enhance(int attackMod, int defenseMod)
        {
            if (attackMod == 0 && defenseMod == 0)
            {
                throw new ArgumentException("Attack and defense must not both be zero.");
            }

            m_attackModifier = attackMod != 0 ? new Warrior.ValueModifier(Warrior.ValueModifier.Operators.Add, attackMod) : null;
            m_defenseModifier = defenseMod != 0 ? new Warrior.ValueModifier(Warrior.ValueModifier.Operators.Add, defenseMod) : null;
        }

        void IEpilogTrigger<AddBehavior>.Run(CommandContext<AddBehavior> context)
        {
            if (context.Command.Behavior == this)
            {
                if (context.Command.Target.Behaviors.Has<Warrior>())
                {
                    if (m_attackModifier != null)
                    {
                        context.Game.IssueCommands(new SendBehaviorMessage
                        {
                            Target = context.Command.Target.Behaviors.Get<Warrior>(),
                            Message = "AttackModifiers",
                            Args = new object[] { "add", m_attackModifier }
                        });
                    }
                    if (m_defenseModifier != null)
                    {
                        context.Game.IssueCommands(new SendBehaviorMessage
                        {
                            Target = context.Command.Target.Behaviors.Get<Warrior>(),
                            Message = "DefenseModifiers",
                            Args = new object[] { "add", m_defenseModifier }
                        });
                    }
                }
                else
                {
                    m_attackModifier = null;
                    m_defenseModifier = null;
                }
            }
        }

        void IPrologTrigger<RemoveBehavior>.Run(CommandContext<RemoveBehavior> context)
        {
            if (context.Command.Behavior == this)
            {
                if (m_attackModifier != null)
                {
                    context.Game.IssueCommands(new SendBehaviorMessage
                    {
                        Target = Host.Behaviors.Get<Warrior>(),
                        Message = "AttackModifiers",
                        Args = new object[] { "remove", m_attackModifier }
                    });
                    m_attackModifier = null;
                }
                if (m_defenseModifier != null)
                {
                    context.Game.IssueCommands(new SendBehaviorMessage
                    {
                        Target = Host.Behaviors.Get<Warrior>(),
                        Message = "DefenseModifiers",
                        Args = new object[] { "remove", m_defenseModifier }
                    });
                    m_defenseModifier = null;
                }
            }
        }
    }
}
