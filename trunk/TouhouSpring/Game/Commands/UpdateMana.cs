using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Commands
{
    public class UpdateMana : BaseCommand
    {
        public Player Player
        {
            get; private set;
        }

        public int DeltaAmount
        {
            get; private set;
        }

        public UpdateMana(Player player, int deltaAmount, ICause cause)
            : base(cause)
        {
            if (player == null)
            {
                throw new ArgumentNullException("player");
            }

            Player = player;
            DeltaAmount = deltaAmount;
        }

        public void PatchDeltaAmount(int value)
        {
            CheckPatchable("DeltaAmount");
            DeltaAmount = value;
        }

        internal override void ValidateOnIssue()
        {
            Validate(Player);
        }

        internal override void ValidateOnRun()
        {
            if (Player.Mana + DeltaAmount < 0)
            {
                FailValidation("Insufficient mana.");
            }
        }

        internal override void RunMain()
        {
            Player.Mana = Math.Min(Player.Mana + DeltaAmount, Player.Hero.Model.Mana);
        }
    }
}
