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
            else if (!Target.Behaviors.Has<Behaviors.Warrior>())
            {
                FailValidation("Damage cannot be dealt to non-warrior cards.");
            }
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
            Target.Behaviors.Get<Behaviors.Warrior>().Life -= DamageToDeal;
        }
    }
}
