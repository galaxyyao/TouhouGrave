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
        public CardInstance Target
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

        public Kill(CardInstance target, ICause cause)
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
                Game.UnsubscribeCardFromCommands(Target);
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

            if (!Target.IsHero)
            {
                Target.Owner.Graveyard.AddToTop(Target.Model);
                Target.IsDestroyed = true;
                EnteredGraveyard = true;
            }
        }
    }
}
