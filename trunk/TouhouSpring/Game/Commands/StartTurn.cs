using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Commands
{
    public class StartTurn : BaseCommand
    {
        public Player Player
        {
            get; private set;
        }

        public StartTurn(Player player)
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
        }
    }
}
