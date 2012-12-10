﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public class HealPlayer : BaseCommand
    {
        public Player Player
        {
            get; private set;
        }

        // TODO: change to some serializable reference
        public Behaviors.IBehavior Cause
        {
            get; private set;
        }

        public int HealAmount
        {
            get; private set;
        }

        public HealPlayer(Player player, Behaviors.IBehavior cause, int healAmount)
        {
            if (player == null)
            {
                throw new ArgumentNullException("player");
            }

            Player = player;
            Cause = cause;
            HealAmount = healAmount;
        }

        public void PatchHealAmount(int value)
        {
            CheckPatchable("HealAmount");
            HealAmount = value;
        }

        internal override void ValidateOnIssue()
        {
            Validate(Player);
            ValidateOrNull(Cause);
        }

        internal override void ValidateOnRun()
        {
            if (HealAmount < 0)
            {
                FailValidation("Amount must be greater than zero.");
            }
        }

        internal override void RunMain()
        {
            Player.Health += Math.Max(HealAmount, 0);
        }
    }
}
