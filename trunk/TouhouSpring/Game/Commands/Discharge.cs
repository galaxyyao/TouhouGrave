using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Commands
{
    public class Discharge : BaseCommand
    {
        public Player Player
        {
            get; private set;
        }

        public Discharge(Player player)
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
            if (!Player.IsSkillCharged)
            {
                FailValidation("Player is already discharged.");
            }
        }

        internal override void RunMain()
        {
            Player.IsSkillCharged = false;
        }
    }
}
