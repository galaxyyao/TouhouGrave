using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public class SimpleBehavior<T> : BaseBehavior<SimpleBehavior<T>.ModelType>
        where T : IBehavior, new()
    {
        public class ModelType : BehaviorModel<T>
        {
        }
    }
}
