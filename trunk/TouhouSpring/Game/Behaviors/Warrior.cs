﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using TouhouSpring.Commands;

namespace TouhouSpring.Behaviors
{
    public enum WarriorState
    {
        StandingBy,
        CoolingDown
    }

    public partial class Warrior : BaseBehavior<Warrior.ModelType>,
        IEpilogTrigger<Kill>
    {
        public WarriorState State
        {
            get; private set;
        }

        public int Attack
        {
            get; private set;
        }

        public int Defense
        {
            get; private set;
        }

        public int AccumulatedDamage
        {
            get; set;
        }

        public List<BaseCard> Equipments
        {
            get; private set;
        }

        void IEpilogTrigger<Kill>.Run(CommandContext<Kill> context)
        {
            if (context.Command.Target == Host)
            {
                State = WarriorState.StandingBy;
                m_attackModifers.Clear();
                m_defenseModifiers.Clear();
                Attack = Model.Attack;
                Defense = Model.Defense;
                AccumulatedDamage = 0;
                Equipments.Clear();
            }
        }

        public override void OnMessage(string message, object[] args)
        {
            if (message == "GoCoolingDown")
            {
                if (args != null)
                {
                    throw new ArgumentException("Formation of args is not expected.");
                }

                State = WarriorState.CoolingDown;
            }
            else if (message == "GoStandingBy")
            {
                if (args != null)
                {
                    throw new ArgumentException("Formation of args is not expected.");
                }

                State = WarriorState.StandingBy;
            }
            else if (message == "AttackModifiers")
            {
                if (args == null || args.Length != 2
                    || args[0].GetType() != typeof(string) || args[1].GetType() != typeof(ValueModifier))
                {
                    throw new ArgumentException("Formation of args is not expected.");
                }
                if ((string)args[0] == "add")
                {
                    var mod = (ValueModifier)args[1];
                    if (m_attackModifers.Contains(mod))
                    {
                        throw new ArgumentException("The modifier has already been added.");
                    }
                    m_attackModifers.Add(mod);
                }
                else if ((string)args[0] == "remove")
                {
                    var mod = (ValueModifier)args[1];
                    if (!m_attackModifers.Contains(mod))
                    {
                        throw new ArgumentException("The modifier has not been added.");
                    }
                    m_attackModifers.Remove(mod);
                }
                Attack = m_attackModifers.Aggregate(Model.Attack, (i, v) => v.Process(i));
            }
            else if (message == "DefenseModifiers")
            {
                if (args == null || args.Length != 2
                    || args[0].GetType() != typeof(string) || args[1].GetType() != typeof(ValueModifier))
                {
                    throw new ArgumentException("Formation of args is not expected.");
                }
                if ((string)args[0] == "add")
                {
                    var mod = (ValueModifier)args[1];
                    if (m_defenseModifiers.Contains(mod))
                    {
                        throw new ArgumentException("The modifier has already been added.");
                    }
                    m_defenseModifiers.Add(mod);
                }
                else if ((string)args[0] == "remove")
                {
                    var mod = (ValueModifier)args[1];
                    if (!m_defenseModifiers.Contains(mod))
                    {
                        throw new ArgumentException("The modifier has not been added.");
                    }
                    m_defenseModifiers.Remove(mod);
                }
                Defense = m_defenseModifiers.Aggregate(Model.Defense, (i, v) => v.Process(i));
            }
        }

        protected override void OnInitialize()
        {
            State = WarriorState.StandingBy;
            Attack = Model.Attack;
            Defense = Model.Defense;
            Equipments = new List<BaseCard>();
        }

        [BehaviorModel(typeof(Warrior), Category = "Core", Description = "The card is capable of being engaged into combats.")]
        public class ModelType : BehaviorModel
        {
            public int Attack { get; set; }
            public int Defense { get; set; }
        }
    }
}
