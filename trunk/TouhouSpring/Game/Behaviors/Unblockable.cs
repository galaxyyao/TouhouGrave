using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Unblockable : BaseBehavior<Unblockable.ModelType>
    {
        [BehaviorModel(typeof(Unblockable), Category = "Core", Description = "The card can't be blocked in combats.")]
        public class ModelType : BehaviorModel
        { }
    }
}
