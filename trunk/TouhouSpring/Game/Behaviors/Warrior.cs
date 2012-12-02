using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public enum WarriorState
    {
        StandingBy,
        CoolingDown
    }

    public class Warrior : BaseBehavior<Warrior.ModelType>,
        ITrigger<Triggers.CardLeftBattlefieldContext>
    {
        public class ValueModifier
        {
            public enum Operators
            {
                Add,
                Multiply,
                DivideRoundUp,
                DivideRoundDown
            }

            public Operators Operator { get; private set; }
            public int Amount { get; private set; }

            public ValueModifier(Operators op, int amount)
            {
                switch (op)
                {
                    case Operators.Add:
                        if (amount == 0)
                        {
                            throw new ArgumentOutOfRangeException("Amount should not be zero.");
                        }
                        break;
                    case Operators.Multiply:
                        if (amount <= 0)
                        {
                            throw new ArgumentOutOfRangeException("Amount should be greater than zero.");
                        }
                        break;
                    case Operators.DivideRoundUp:
                    case Operators.DivideRoundDown:
                        if (amount <= 0)
                        {
                            throw new ArgumentOutOfRangeException("Amount should be greater than zero.");
                        }
                        break;
                    default:
                        throw new ArgumentException("op");
                }

                Operator = op;
                Amount = amount;
            }

            internal int Process(int input)
            {
                switch (Operator)
                {
                    case Operators.Add:
                        return Math.Max(input + Amount, 0);
                    case Operators.Multiply:
                        return Math.Max(input * Amount, 0);
                    case Operators.DivideRoundUp:
                        return Math.Max((int)Math.Ceiling((float)input / Amount), 0);
                    case Operators.DivideRoundDown:
                        return Math.Max((int)Math.Floor((float)input / Amount), 0);
                    default:
                        throw new ArgumentException("Operator");
                }
            }
        }

        private List<ValueModifier> m_attackModifers = new List<ValueModifier>();
        private List<ValueModifier> m_defenseModifiers = new List<ValueModifier>();

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

        public void Trigger(Triggers.CardLeftBattlefieldContext context)
        {
            if (context.CardToLeft == Host)
            {
                State = WarriorState.StandingBy;
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
