﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Commands
{
    /// <summary>
    /// Shuffle a player's library
    /// </summary>
    public class ShuffleLibrary : BaseCommand
    {
        public Player Player
        {
            get; private set;
        }

        public ShuffleLibrary(Player player)
        {
            if (player == null)
            {
                throw new ArgumentNullException("player");
            }

            Player = player;
        }

        internal override void ValidateOnIssue()
        {
            Validate(Player);
        }

        internal override bool ValidateOnRun()
        {
            return true;
        }

        internal override void RunMain()
        {
            Player.Library.Shuffle(Context.Game.Random);
        }
    }
}
