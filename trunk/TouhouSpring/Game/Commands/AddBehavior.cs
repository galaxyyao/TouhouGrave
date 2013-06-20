using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Commands
{
    public class AddBehavior : BaseCommand
    {
        // TODO: change to serialization-friendly ID
        public CardInstance Target
        {
            get; private set;
        }

        // TODO: change to serialization-friendly ID
        public Behaviors.IBehavior Behavior
        {
            get; private set;
        }

        public AddBehavior(CardInstance target, Behaviors.IBehavior behavior)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }
            else if (behavior == null)
            {
                throw new ArgumentNullException("behavior");
            }

            Target = target;
            Behavior = behavior;
        }

        internal override void ValidateOnIssue()
        {
            Validate(Target);
            if (Behavior == null)
            {
                FailValidation("Behavior to be added can't be null.");
            }
            else if (Behavior.Host != null)
            {
                FailValidation("Behavior can't be bound already.");
            }
            else if (Behavior.Persistent)
            {
                FailValidation("Persistent behavior can't be added dynamically.");
            }
        }

        internal override bool ValidateOnRun()
        {
            return !Target.IsDestroyed && !Target.Behaviors.Contains(Behavior);
        }

        internal override void RunMain()
        {
            Target.Behaviors.Add(Behavior);
        }
    }
}
