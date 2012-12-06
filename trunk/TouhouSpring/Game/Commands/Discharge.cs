using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Commands
{
    public class Discharge : ICommand
    {
        public string Token
        {
            get { return "Discharge"; }
        }

        public Player Player
        {
            get; set;
        }

        public void Validate(Game game)
        {
            if (Player == null)
            {
                throw new CommandValidationFailException("Player can't be null.");
            }
            else if (!game.Players.Contains(Player))
            {
                throw new CommandValidationFailException("The Player is not registered in game.");
            }
            else if (!Player.IsSkillCharged)
            {
                throw new CommandValidationFailException("Player is already discharged.");
            }
        }

        public void RunMain(Game game)
        {
            Player.IsSkillCharged = false;
        }
    }
}
