using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Commands
{
    public class EndPhase : BaseCommand
    {
        public string PreviousPhase
        {
            get; private set;
        }

        public EndPhase()
        { }

        internal override void ValidateOnIssue()
        {
        }

        internal override bool ValidateOnRun()
        {
            if (Context.Game.CurrentPhase == "")
            {
                FailValidation("EndPhase must be issued only when the game is in some phase.");
            }
            return true;
        }

        internal override void RunMain()
        {
            PreviousPhase = Context.Game.CurrentPhase;
            Context.Game.CurrentPhase = "";
        }
    }
}
