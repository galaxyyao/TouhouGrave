using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public class Unselectable : BaseBehavior<Unselectable.ModelType>
    {
        [BehaviorModel(typeof(Unselectable), Category = "Core")]
        public class ModelType : BehaviorModel
        { }
    }
}
