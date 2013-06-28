using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public class Unattackable : BaseBehavior<Unattackable.ModelType>
    {
        [BehaviorModel(typeof(Unattackable), Category = "Core")]
        public class ModelType : BehaviorModel
        { }
    }
}
