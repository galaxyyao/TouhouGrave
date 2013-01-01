using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TouhouSpring.Commands
{
    public class Kill : BaseCommand
    {
        // TODO: change to serialization-friendly ID
        public BaseCard Target
        {
            get; private set;
        }

        public bool LeftBattlefield
        {
            get; private set;
        }

        public bool EnteredGraveyard
        {
            get; private set;
        }

        public Kill(BaseCard target, ICause cause)
            : base(cause)
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
            if (!Target.Owner.CardsOnBattlefield.Contains(Target)
                && !Target.Owner.CardsOnHand.Contains(Target))
            {
                FailValidation("Target card is not on battlefield nor on hand.");
            }
        }

        internal override void ValidateOnRun()
        {
        }

        internal override void RunMain()
        {
            if (Target.Owner.CardsOnBattlefield.Contains(Target))
            {
                Target.Owner.m_battlefieldCards.Remove(Target);
                LeftBattlefield = true;
            }
            else if (Target.Owner.CardsOnHand.Contains(Target))
            {
                Target.Owner.m_handSet.Remove(Target);
            }
            else
            {
                Debug.Assert(false);
            }

            // reset card states
            for (int i = 0; i < Target.Behaviors.Count; ++i)
            {
                if (!Target.Behaviors[i].Persistent)
                {
                    Target.Behaviors.RemoveAt(i);
                    --i;
                }
                else
                {
                    // TODO: reset behavior states
                }
            }

            if (!Target.IsHero)
            {
                Target.Owner.Graveyard.AddCardToTop(Target);
                EnteredGraveyard = true;
            }
        }
    }
}
