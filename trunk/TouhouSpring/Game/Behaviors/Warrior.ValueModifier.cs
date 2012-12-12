using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed partial class Warrior
    {
        public enum ValueModifierOperator
        {
            Add,
            Multiply,
            DivideRoundUp,
            DivideRoundDown
        }

        public class ValueModifier
        {
            public ValueModifierOperator Operator { get; private set; }
            public int Amount { get; private set; }

            public ValueModifier(ValueModifierOperator op, int amount)
            {
                switch (op)
                {
                    case ValueModifierOperator.Add:
                        if (amount == 0)
                        {
                            throw new ArgumentOutOfRangeException("amount", "Amount should not be zero.");
                        }
                        break;
                    case ValueModifierOperator.Multiply:
                        if (amount <= 0)
                        {
                            throw new ArgumentOutOfRangeException("amount", "Amount should be greater than zero.");
                        }
                        break;
                    case ValueModifierOperator.DivideRoundUp:
                    case ValueModifierOperator.DivideRoundDown:
                        if (amount <= 0)
                        {
                            throw new ArgumentOutOfRangeException("amount", "Amount should be greater than zero.");
                        }
                        break;
                    default:
                        throw new ArgumentException("Invalid value.", "op");
                }

                Operator = op;
                Amount = amount;
            }

            internal int Process(int input)
            {
                switch (Operator)
                {
                    case ValueModifierOperator.Add:
                        return Math.Max(input + Amount, 0);
                    case ValueModifierOperator.Multiply:
                        return Math.Max(input * Amount, 0);
                    case ValueModifierOperator.DivideRoundUp:
                        return Math.Max((int)Math.Ceiling((float)input / Amount), 0);
                    case ValueModifierOperator.DivideRoundDown:
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
