using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring
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
            : this(op, amount, true)
        { }

        public ValueModifier(ValueModifierOperator op, int amount, bool throwException)
        {
            switch (op)
            {
                case ValueModifierOperator.Add:
                    break;
                case ValueModifierOperator.Multiply:
                    if (amount <= 0)
                    {
                        if (throwException)
                        {
                            throw new ArgumentOutOfRangeException("amount", "Amount should be greater than zero.");
                        }
                        else
                        {
                            amount = 1;
                        }
                    }
                    break;
                case ValueModifierOperator.DivideRoundUp:
                case ValueModifierOperator.DivideRoundDown:
                    if (amount <= 0)
                    {
                        if (throwException)
                        {
                            throw new ArgumentOutOfRangeException("amount", "Amount should be greater than zero.");
                        }
                        else
                        {
                            amount = 1;
                        }
                    }
                    break;
                default:
                    throw new ArgumentException("Invalid value.", "op");
            }

            Operator = op;
            Amount = amount;
        }

        public int Process(int input)
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
}
