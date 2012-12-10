using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Commands
{
    public class StartTurn : BaseCommand
    {
        internal override void ValidateOnIssue()
        {
        }

        internal override void ValidateOnRun()
        {
            if (Game.CurrentPhase != "PhaseA")
            {
                FailValidation(String.Format("StartTurn can't be executed at the phase {0}.", Game.CurrentPhase));
            }
        }

        internal override void RunMain()
        {
            Game.PlayerPlayer.m_battlefieldCards
                .Where(card => card.Behaviors.Has<Behaviors.Warrior>())
                .ForEach(card => Game.IssueCommands(
                    new Commands.SendBehaviorMessage(card.Behaviors.Get<Behaviors.Warrior>(), "GoStandingBy", null)));
        }
    }
}
