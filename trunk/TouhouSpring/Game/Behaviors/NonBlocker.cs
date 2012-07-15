using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public class NonBlocker : BaseBehavior<NonBlocker.ModelType>
    {
        [BehaviorModel("NonBlocker", typeof(NonBlocker), Description = "The card can't be engaged into combats as a blocker.")]
        public class ModelType : BehaviorModel
        { }
    }
}
