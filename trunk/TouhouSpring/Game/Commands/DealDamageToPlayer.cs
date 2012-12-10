using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Commands
{
    public class DealDamageToPlayer : BaseCommand
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

        public int DamageToDeal
        {
            get; private set;
        }

        public DealDamageToPlayer(Player player, Behaviors.IBehavior cause, int damageToDeal)
        {
            if (player == null)
            {
                throw new ArgumentNullException("player");
            }

            Player = player;
            Cause = cause;
            DamageToDeal = damageToDeal;
        }

        public void PatchDamageToDeal(int value)
        {
            CheckPatchable("DamageToDeal");
            DamageToDeal = value;
        }

        internal override void ValidateOnIssue()
        {
            Validate(Player);
            ValidateOrNull(Cause);
        }

        internal override void ValidateOnRun()
        {
            if (DamageToDeal < 0)
            {
                FailValidation("Damage must be greater than zero.");
            }
        }

        internal override void RunMain()
        {
            Player.Health -= Math.Max(DamageToDeal, 0);
        }
    }
}
