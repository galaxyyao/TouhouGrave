using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Commands
{
    public class SubtractPlayerLife : BaseCommand
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

        public SubtractPlayerLife(Player player, int amount, ICause cause)
            : this(player, amount, false, cause)
        { }

        internal SubtractPlayerLife(Player player, int amount, bool ignoreModifiers, ICause cause)
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
            FinalAmount = ignoreModifiers ? amount : player.CalculateFinalLifeSubtract(amount);
        }

        internal override void ValidateOnIssue()
        {
            Validate(Player);
        }

        internal override void ValidateOnRun()
        {
        }

        internal override void RunMain()
        {
            Player.Health -= FinalAmount;
        }
    }
}
