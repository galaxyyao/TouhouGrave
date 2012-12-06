using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Commands
{
    public class EndTurn : ICommand
    {
        public string Token
        {
            get { return "EndTurn"; }
        }

        public void Validate(Game game)
        {
        }

        public void RunMain(Game game)
        {
            if (game.CurrentPhase != "Combat/Resolve")
            {
                throw new InvalidOperationException(String.Format("EndTurn can't be executed at the phase {0}.", game.CurrentPhase));
            }

            game.PlayerPlayer.ResetManaDelta();
        }
    }
}
