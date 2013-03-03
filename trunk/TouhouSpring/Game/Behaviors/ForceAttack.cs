using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public class ForceAttack : BaseBehavior<ForceAttack.ModelType>, IStatusEffect
    {
        public string IconUri { get { return "atlas:Textures/Icons/Icons0$frenzy"; } }
        public string Text { get { return "狂热\n该角色必须进攻。"; } }

        [BehaviorModel(typeof(ForceAttack), Category = "Core", Description = "Card must attack if it can.")]
        public class ModelType : BehaviorModel
        { }
    }
}
