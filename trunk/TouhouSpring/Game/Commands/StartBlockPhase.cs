using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Commands
{
    public class StartBlockPhase : BaseCommand
    {
        internal override void ValidateOnIssue()
        {
        }

        internal override void ValidateOnRun()
        {
            if (Game.CurrentPhase != "Combat/Block")
            {
                FailValidation(String.Format("StartTurn can't be executed at the phase {0}.", Game.CurrentPhase));
            }
        }

        internal override void RunMain()
        {
        }
    }
}
