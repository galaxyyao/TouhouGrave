using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Commands
{
    public class StartPhase : BaseCommand
    {
        public string PhaseName
        {
            get; private set;
        }

        public StartPhase(string phaseName)
        {
            if (String.IsNullOrEmpty(phaseName))
            {
                throw new ArgumentNullException("phaseName");
            }

            PhaseName = phaseName;
        }

        internal override void ValidateOnIssue()
        {
        }

        internal override void ValidateOnRun()
        {
            if (Game.CurrentPhase != "")
            {
                FailValidation("StartPhase must be issued only when the game is not in any other phase.");
            }
        }

        internal override void RunMain()
        {
            Game.CurrentPhase = PhaseName;
        }
    }
}
