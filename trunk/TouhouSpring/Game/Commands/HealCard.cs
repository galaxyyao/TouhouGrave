using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Commands
{
    public class HealCard : BaseCommand
    {
        public BaseCard Target
        {
            get;
            private set;
        }

        public int LifeToHeal
        {
            get;
            private set;
        }

        public HealCard(BaseCard target, int lifeToHeal, ICause cause)
            : base(cause)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }

            Target = target;
            LifeToHeal = lifeToHeal;
        }

        internal override void ValidateOnIssue()
        {
            Validate(Target);
            if (!Target.Owner.CardsOnBattlefield.Contains(Target))
            {
                FailValidation("Life can only be healed to cards on battlefield.");
            }
            else if (!Target.Behaviors.Has<Behaviors.Warrior>())
            {
                FailValidation("Life can not be healed to non-warrior cards.");
            }
        }

        internal override void ValidateOnRun()
        {
            if (LifeToHeal < 0)
            {
                FailValidation("Life to heal must be greater than zero.");
            }
        }

        internal override void RunMain()
        {
            var warrior = Target.Behaviors.Get<Behaviors.Warrior>();
            warrior.Life = (warrior.Life + LifeToHeal) > warrior.InitialLife
                ? warrior.InitialLife
                : (warrior.Life + LifeToHeal);
        }
    }
}
