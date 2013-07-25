using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TouhouSpring.Behaviors;

namespace TouhouSpring.THSv0_5.Effects
{
    public sealed class PoisonousArrow : Utilities.SuppressHeal.Behavior<PoisonousArrow.ModelType>,
        IStatusEffect
    {
        string IStatusEffect.IconUri
        {
            get { return "atlas:Textures/Icons/Icons0$BTNEnvenomedSpear"; }
        }

        string IStatusEffect.Text
        {
            get { return "毒箭\n无法受到治疗。"; }
        }

        [BehaviorModel(typeof(PoisonousArrow), Category = "v0.5/Effects", DefaultName = "毒箭（效果）")]
        public class ModelType : Utilities.SuppressHeal.ModelType
        { }
    }
}
