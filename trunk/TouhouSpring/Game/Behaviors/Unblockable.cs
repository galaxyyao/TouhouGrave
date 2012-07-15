using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    class Unblockable : BaseBehavior<Unblockable.ModelType>
    {
        [BehaviorModel("Unblockable", typeof(Unblockable), Description = "The card can't be blocked in combats.")]
        public class ModelType : BehaviorModel
        { }
    }
}
