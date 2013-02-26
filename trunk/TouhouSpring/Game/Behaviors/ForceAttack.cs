using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public class ForceAttack : BaseBehavior<ForceAttack.ModelType>
    {
        [BehaviorModel(Category = "Core", Description = "Card must attack if it can.")]
        public class ModelType : BehaviorModel<ForceAttack>
        { }
    }
}
