using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Undefendable : BaseBehavior<Undefendable.ModelType>
    {
        [BehaviorModel(typeof(Undefendable), Category = "Core")]
        public class ModelType : BehaviorModel
        { }
    }
}
