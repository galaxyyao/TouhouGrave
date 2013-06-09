using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Commands
{
    public class DeactivateAssist : BaseCommand
    {
        // TODO: change to serialization-friendly ID
        public CardInstance CardToDeactivate
        {
            get; private set;
        }

        public DeactivateAssist(CardInstance cardToDeactivate)
        {
            if (cardToDeactivate == null)
            {
                throw new ArgumentNullException("cardToDeactivate");
            }

            CardToDeactivate = cardToDeactivate;
        }

        internal override void ValidateOnIssue()
        {
            Validate(CardToDeactivate);
            if (!CardToDeactivate.Owner.Assists.Contains(CardToDeactivate))
            {
                FailValidation("Not a valid assist card.");
            }
            else if (!CardToDeactivate.Owner.ActivatedAssits.Contains(CardToDeactivate))
            {
                FailValidation("Card is not activated.");
            }
        }

        internal override bool ValidateOnRun()
        {
            return !CardToDeactivate.IsDestroyed && CardToDeactivate.Owner.ActivatedAssits.Contains(CardToDeactivate);
        }

        internal override void RunMain()
        {
            CardToDeactivate.Owner.m_activatedAssists.Remove(CardToDeactivate);
            Context.Game.UnsubscribeCardFromCommands(CardToDeactivate);
        }
    }
}
