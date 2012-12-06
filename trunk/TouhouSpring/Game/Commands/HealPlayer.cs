using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public class HealPlayer : ICommand
    {
        public string Token
        {
            get { return "HealPlayer"; }
        }

        public Player Player
        {
            get; set;
        }

        public int Amount
        {
            get; set;
        }

        public Behaviors.IBehavior Cause
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
                throw new CommandValidationFailException("The Player object is not registered in game.");
            }
            else if (Amount <= 0)
            {
                throw new CommandValidationFailException("Amount must be greater than zero.");
            }
        }

        public void RunMain(Game game)
        {
            Player.Health += Math.Max(Amount, 0);
        }
    }
}
