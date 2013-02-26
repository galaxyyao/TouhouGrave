using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Commands
{
    public class RemoveBehavior : BaseCommand
    {
        // TODO: change to serialization-friendly ID
        public BaseCard Target
        {
            get; private set;
        }

        // TODO: change to serialization-friendly ID
        public Behaviors.IBehavior Behavior
        {
            get; private set;
        }

        public RemoveBehavior(BaseCard target, Behaviors.IBehavior behavior)
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
            Validate(Behavior);
        }

        internal override void ValidateOnRun()
        {
        }

        internal override void RunMain()
        {
            Target.Behaviors.Remove(Behavior);
            Game.UnsubscribeBehaviorFromCommands(Target, Behavior);
        }
    }
}
