using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Commands
{
    public class AddCounter : BaseCommand
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

        public int NumToAdd
        {
            get; private set;
        }

        public AddCounter(CardInstance target, Behaviors.ICounter counter)
            : this(target, counter, 1)
        { }

        public AddCounter(CardInstance target, Behaviors.ICounter counter, int numToAdd)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }
            else if (counter == null)
            {
                throw new ArgumentNullException("behavior");
            }
            else if (numToAdd <= 0)
            {
                throw new ArgumentOutOfRangeException("numToAdd", "NumToAdd must be greater than zero.");
            }

            Target = target;
            Counter = counter;
            NumToAdd = numToAdd;
        }

        internal override void ValidateOnIssue()
        { }

        internal override bool ValidateOnRun()
        {
            return !Target.IsDestroyed && NumToAdd >= 0 && (Target.IsOnBattlefield || Target.IsActivatedAssist);
        }

        internal override void RunMain()
        {
            Target.AddCounter(Counter, NumToAdd);
        }
    }
}
