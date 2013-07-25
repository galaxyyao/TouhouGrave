using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public class Trap : BaseBehavior<Trap.ModelType>
    {
        [BehaviorModel(typeof(Trap), Category = "Core", DefaultName = "陷阱")]
        public class ModelType : BehaviorModel
        { }
    }
}
