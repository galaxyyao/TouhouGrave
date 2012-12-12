using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class NonBlocker : BaseBehavior<NonBlocker.ModelType>
    {
        [BehaviorModel(typeof(NonBlocker), Category = "Core", Description = "The card can't be engaged into combats as a blocker.")]
        public class ModelType : BehaviorModel
        { }
    }
}
