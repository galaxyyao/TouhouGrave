using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Commands
{
    public class StartTurn : ICommand
    {
        public string Token
        {
            get { return "StartTurn"; }
        }

        public void Validate(Game game)
        {
        }

        public void RunMain(Game game)
        {
            if (game.CurrentPhase != "PhaseA")
            {
                throw new InvalidOperationException(String.Format("StartTurn can't be executed at the phase {0}.", game.CurrentPhase));
            }

            game.PlayerPlayer.m_battlefieldCards
                .Where(card => card.Behaviors.Has<Behaviors.Warrior>())
                .ForEach(card => game.IssueCommands(new Commands.SendBehaviorMessage
                {
                    Target = card.Behaviors.Get<Behaviors.Warrior>(),
                    Message = "GoStandingBy"
                }));
        }
    }
}
