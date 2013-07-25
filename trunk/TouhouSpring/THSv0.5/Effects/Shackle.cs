using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TouhouSpring.Behaviors;

namespace TouhouSpring.THSv0_5.Effects
{
    public sealed class Shackle : BaseBehavior<Shackle.ModelType>,
        IStatusEffect,
        IUnattackable, IUnretaliatable
    {
        string IStatusEffect.IconUri
        {
            get { return "atlas:Textures/Icons/Icons0$BTNMagicLariet"; }
        }

        string IStatusEffect.Text
        {
            get { return "枷锁\n无法攻击。无法反击。"; }
        }

        [BehaviorModel(typeof(Shackle), Category = "v0.5/Effects", DefaultName = "枷锁（效果）")]
        public class ModelType : BehaviorModel
        { }
    }
}
