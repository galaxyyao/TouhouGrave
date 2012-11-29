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

        public bool PreReserved
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
            else if (PreReserved && Amount > 0)
            {
                throw new CommandValidationFailException("Amount must be negative value if the mana is pre-reserved.");
            }
            else if (!PreReserved && Player.FreeMana + Amount < 0)
            {
                throw new CommandValidationFailException("Insufficient mana.");
            }
            else if (PreReserved && Player.ReservedMana < -Amount)
            {
                throw new CommandValidationFailException("Invalid mana reservation.");
            }
        }

        public void RunMain(Game game)
        {
            Player.Mana = Math.Min(Player.Mana + Amount, Player.Hero.Model.Mana);
            if (PreReserved)
            {
                Player.ReservedMana += Amount;
            }
        }
    }
}
