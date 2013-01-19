using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Commands
{
    public class EndTurn : BaseCommand
    {
        public Player Player
        {
            get; private set;
        }

        public EndTurn(Player player)
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

        internal override void ValidateOnRun()
        {
        }

        internal override void RunMain()
        {
        }
    }
}
