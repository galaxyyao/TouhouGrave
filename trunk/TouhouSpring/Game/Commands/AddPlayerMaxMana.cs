﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Commands
{
    public class AddPlayerMaxMana : BaseCommand
    {
        public Player Player
        {
            get;
            private set;
        }

        public int Amount
        {
            get;
            private set;
        }

        public int FinalAmount
        {
            get;
            private set;
        }

        public AddPlayerMaxMana(Player player, int amount, ICause cause)
            : this(player, amount, false, cause)
        { }

        public AddPlayerMaxMana(Player player, int amount, bool ignoreModifiers, ICause cause)
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
            FinalAmount = ignoreModifiers ? amount : player.CalculateFinalManaAdd(amount);
        }

        internal override void ValidateOnIssue()
        {
            Validate(Player);
        }

        internal override bool ValidateOnRun()
        {
            return FinalAmount >= 0;
        }

        internal override void RunMain()
        {
            Player.MaxMana += FinalAmount;
        }
    }
}