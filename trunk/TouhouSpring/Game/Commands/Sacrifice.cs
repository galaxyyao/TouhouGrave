﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Commands
{
    public class Sacrifice : BaseCommand, IInitiativeCommand
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

        public Sacrifice(CardInstance target)
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
            if (!Target.Owner.CardsOnHand.Contains(Target))
            {
                FailValidation("Card can only be sacrificed from hand.");
            }
        }

        internal override bool ValidateOnRun()
        {
            return !Target.IsDestroyed && Target.Owner.CardsOnHand.Contains(Target);
        }

        internal override void RunMain()
        {
            Target.Owner.m_handSet.Remove(Target);
            Target.Owner.AddToSacrificeSorted(Target);
        }
    }
}
