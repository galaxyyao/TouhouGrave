using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public partial class Warrior
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
    }
}
