using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_AttackGrowth : BaseBehavior<Passive_AttackGrowth.ModelType>
    {

        [BehaviorModel(typeof(Passive_AttackGrowth), DefaultName = "")]
        public class ModelType : BehaviorModel
        {
        }
    }
}
