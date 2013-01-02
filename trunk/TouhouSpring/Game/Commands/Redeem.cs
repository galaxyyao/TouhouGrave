using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Commands
{
    public class Redeem : BaseCommand
    {
        // TODO: change to serialization-friendly ID
        public BaseCard Target
        {
            get; private set;
        }

        public Redeem(BaseCard target)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }

            Target = target;
        }

        internal override void ValidateOnIssue()
        {
            Validate(Target);
            if (!Target.Owner.CardsSacrificed.Contains(Target))
            {
                FailValidation("Only sacrifices can be redeemed.");
            }
        }

        internal override void ValidateOnRun()
        {
        }

        internal override void RunMain()
        {
            Target.Owner.m_sacrifices.Remove(Target);
            Target.Owner.AddToHandSorted(Target);
        }
    }
}
