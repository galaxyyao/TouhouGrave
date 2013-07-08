using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TouhouSpring.Behaviors;

namespace TouhouSpring.THSv0_5.Passives
{
    public sealed class Taunt : BaseBehavior<Taunt.ModelType>,
        ITaunt, IStatusEffect
    {
        public string IconUri { get { return Model.IconUri; } }
        public string Text { get { return Model.Text; } }

        [BehaviorModel(typeof(Taunt), Category = "v0.5/Passive", DefaultName = "嘲讽（图标）")]
        public class ModelType : BehaviorModel
        {
            public string IconUri { get; set; }

            // TODO: specify multiline editor
            public string Text { get; set; }

            public ModelType()
            {
                IconUri = "atlas:Textures/Icons/Icons0$BTNTaunt";
                Text = "嘲讽\n对方角色卡必须对本卡发动攻击。";
            }
        }
    }
}
