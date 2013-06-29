using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_Taunt : Taunt, IStatusEffect
    {
        public string IconUri { get { return "atlas:Textures/Icons/Icons0$BTNTaunt"; } }
        public string Text { get { return "嘲讽\n对方角色卡必须对本卡发动攻击。"; } }

        [BehaviorModel(typeof(Passive_Taunt), Category = "v0.5/Passive", DefaultName = "嘲讽（图标）")]
        new public class ModelType : Taunt.ModelType
        { }
    }
}
