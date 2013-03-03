using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Protector : BaseBehavior<Protector.ModelType>
    {
        [BehaviorModel(typeof(Protector), Category = "Core")]
        public class ModelType : BehaviorModel
        { }
    }
}
