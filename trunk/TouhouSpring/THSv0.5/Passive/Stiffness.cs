using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors.Passive
{
    public sealed class Stiffness : BaseBehavior<Stiffness.ModelType>, IUnretaliatable, IStatusEffect
    {
        public string IconUri { get { return "atlas:Textures/Icons/Icons0$BTNAnimateDead"; } }
        public string Text { get { return "僵硬\n无法反击。"; } }

        [BehaviorModel(typeof(Stiffness), Category = "v0.5/Passive", DefaultName = "僵硬")]
        public class ModelType : BehaviorModel
        { }
    }
}
