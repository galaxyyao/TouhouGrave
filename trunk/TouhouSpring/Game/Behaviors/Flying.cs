using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Flying :
        BaseBehavior<Flying.ModelType>,
        IUndefendable,
        IStatusEffect
    {
        public string IconUri { get { return "atlas:Textures/Icons/Icons0$wing"; } }
        public string Text { get { return "飞行状态"; } }

        [BehaviorModel(typeof(Flying), Category = "Core", DefaultName = "飞行状态")]
        public class ModelType : BehaviorModel { }
    }
}
