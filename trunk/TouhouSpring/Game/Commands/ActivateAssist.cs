﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Commands
{
    public class ActivateAssist : BaseCommand
    {
        // TODO: change to serialization-friendly ID
        public BaseCard CardToActivate
        {
            get; private set;
        }

        public ActivateAssist(BaseCard cardToActivate)
        {
            if (cardToActivate == null)
            {
                throw new ArgumentNullException("cardToActivate");
            }

            CardToActivate = cardToActivate;
        }

        internal override void ValidateOnIssue()
        {
            Validate(CardToActivate);
            if (!CardToActivate.Owner.Assists.Contains(CardToActivate))
            {
                FailValidation("Not a valid assist card.");
            }
            else if (CardToActivate.Owner.ActivatedAssist == CardToActivate)
            {
                FailValidation("Can't activate the card twice.");
            }
        }

        internal override void ValidateOnRun()
        {
        }

        internal override void RunMain()
        {
            CardToActivate.Owner.ActivatedAssist = CardToActivate;
        }
    }
}
