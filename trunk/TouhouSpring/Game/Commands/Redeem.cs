﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Commands
{
    public class Redeem : BaseCommand, IInitiativeCommand
    {
        // TODO: change to serialization-friendly ID
        public CardInstance Target
        {
            get; private set;
        }

        public Player Initiator
        {
            get { return Target.Owner; }
        }

        public Redeem(CardInstance target)
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

        internal override bool ValidateOnRun()
        {
            return !Target.IsDestroyed && Target.Owner.CardsSacrificed.Contains(Target);
        }

        internal override void RunMain()
        {
            Target.Owner.m_sacrifices.Remove(Target);
            Target.Owner.AddToHandSorted(Target);
        }
    }
}
