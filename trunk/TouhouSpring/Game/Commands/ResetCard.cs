using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Commands
{
    public class ResetCard : BaseCommand, ISilentCommand
    {
        public CardInstance CardToReset
        {
            get; private set;
        }

        public ResetCard(CardInstance cardToReset)
            : this(cardToReset, null)
        { }

        public ResetCard(CardInstance cardToReset, ICause cause)
            : base(cause)
        {
            if (cardToReset == null)
            {
                throw new ArgumentNullException("cardToReset");
            }

            CardToReset = cardToReset;
        }

        internal override void ValidateOnIssue()
        {
            Validate(CardToReset);
        }

        internal override bool ValidateOnRun()
        {
            return true;
        }

        internal override void RunMain()
        {
            CardToReset.Reset(null);
        }
    }
}
