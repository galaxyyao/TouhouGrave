using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Commands
{
    public class RemoveCounter : BaseCommand, ISilentCommand
    {
        // TODO: change to serialization-friendly ID
        public CardInstance Target
        {
            get; private set;
        }

        public Behaviors.ICounter Counter
        {
            get; private set;
        }

        public int NumToRemove
        {
            get; private set;
        }

        public RemoveCounter(CardInstance target, Behaviors.ICounter counter)
            : this(target, counter, 1)
        { }

        public RemoveCounter(CardInstance target, Behaviors.ICounter counter, int numToRemove)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }
            else if (counter == null)
            {
                throw new ArgumentNullException("behavior");
            }
            else if (numToRemove <= 0)
            {
                throw new ArgumentOutOfRangeException("numToRemove", "NumToRemove must be greater than zero.");
            }

            Target = target;
            Counter = counter;
            NumToRemove = numToRemove;
        }

        internal override void ValidateOnIssue()
        {
            if (Target.GetCounterCount(Counter.GetType()) < NumToRemove)
            {
                FailValidation("Insufficient counters to be removed.");
            }
        }

        internal override bool ValidateOnRun()
        {
            return !Target.IsDestroyed && NumToRemove >= 0 && (Target.IsOnBattlefield || Target.IsActivatedAssist);
        }

        internal override void RunMain()
        {
            Target.RemoveCounter(Counter, NumToRemove);
        }
    }
}
