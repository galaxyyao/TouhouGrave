using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TouhouSpring.Commands
{
    public class DealDamageToCard : BaseCommand
    {
        // TODO: change to some serializable reference
        public CardInstance Target
        {
            get; private set;
        }

        public int DamageToDeal
        {
            get; private set;
        }

        public DealDamageToCard(CardInstance target, int damageToDeal, ICause cause)
            : base(cause)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }

            Target = target;
            DamageToDeal = damageToDeal;
        }

        public void PatchDamageToDeal(int value)
        {
            CheckPatchable("DamageToDeal");
            DamageToDeal = value;
        }

        internal override void ValidateOnIssue()
        {
            Validate(Target);
            if (!Target.Owner.CardsOnBattlefield.Contains(Target))
            {
                FailValidation("Damage can only be dealt to cards on battlefield.");
            }
            else if (Target.Warrior == null)
            {
                FailValidation("Damage cannot be dealt to non-warrior cards.");
            }
        }

        internal override bool ValidateOnRun()
        {
            return DamageToDeal >= 0 && !Target.IsDestroyed && Target.IsOnBattlefield;
        }

        internal override void RunMain()
        {
            Target.Warrior.Life -= DamageToDeal;
        }
    }
}
