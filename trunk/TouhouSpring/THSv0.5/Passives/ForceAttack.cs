using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TouhouSpring.Behaviors;

namespace TouhouSpring.THSv0_5.Passives
{
    public sealed class ForceAttack : BaseBehavior<ForceAttack.ModelType>,
        IForceAttack, IStatusEffect
    {
        public string IconUri { get { return Model.IconUri; } }
        public string Text { get { return Model.Text; } }

        [BehaviorModel(typeof(ForceAttack), Category = "v0.5/Passive", DefaultName = "强制攻击（图标）")]
        public class ModelType : BehaviorModel
        {
            public string IconUri { get; set; }

            // TODO: specify multiline editor
            public string Text { get; set; }

            public ModelType()
            {
                IconUri = "atlas:Textures/Icons/Icons0$frenzy";
                Text = "狂热\n该角色必须进攻。";
            }
        }
    }
}
