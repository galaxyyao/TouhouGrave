using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public class Taunt : BaseBehavior<Taunt.ModelType>
    {
        [BehaviorModel(typeof(Taunt), Category = "Core")]
        public class ModelType : BehaviorModel
        { }
    }
}
