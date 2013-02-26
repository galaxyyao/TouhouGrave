using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Commands
{
    public class SubtractPlayerMana : BaseCommand
    {
        public Player Player
        {
            get; private set;
        }

        public int Amount
        {
            get; private set;
        }

        public int FinalAmount
        {
            get; private set;
        }

        public SubtractPlayerMana(Player player, int amount, ICause cause)
            : this(player, amount, false, cause)
        { }

        public SubtractPlayerMana(Player player, int amount, bool ignoreModifiers, ICause cause)
            : base(cause)
        {
            if (player == null)
            {
                throw new ArgumentNullException("player");
            }
            else if (amount < 0)
            {
                throw new ArgumentOutOfRangeException("amount", "Amount must be greater than or equal to zero.");
            }

            Player = player;
            Amount = amount;
            FinalAmount = ignoreModifiers ? amount : player.CalculateFinalManaSubtract(amount);
        }

        internal override void ValidateOnIssue()
        {
            Validate(Player);
        }

        internal override void ValidateOnRun()
        {
            if (Player.Mana < FinalAmount)
            {
                FailValidation("Insufficient mana.");
            }
        }

        internal override void RunMain()
        {
            Player.Mana -= FinalAmount;
        }
    }
}
