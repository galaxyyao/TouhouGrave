using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public class UpdateMana : ICommand
    {
        public string Token
        {
            get { return "UpdateMana"; }
        }

        public Player Player
        {
            get; set;
        }

        public int Amount
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
            else if (Amount == 0)
            {
                throw new CommandValidationFailException("Mana will not be updated.");
            }
            else if (Player.Mana + Amount < 0)
            {
                throw new CommandValidationFailException("Insufficient mana.");
            }
        }

        public void RunMain(Game game)
        {
            Player.Mana = Math.Min(Player.Mana + Amount, Player.Hero.Model.Mana);
        }
    }
}
