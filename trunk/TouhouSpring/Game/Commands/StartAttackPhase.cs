using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Commands
{
    public class StartAttackPhase : ICommand
    {
        public string Token
        {
            get { return "StartAttackPhase"; }
        }

        public void Validate(Game game)
        {
        }

        public void RunMain(Game game)
        {
            if (game.CurrentPhase != "Combat/Attack")
            {
                throw new InvalidOperationException(String.Format("StartTurn can't be executed at the phase {0}.", game.CurrentPhase));
            }
        }
    }
}
