using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors.Passive
{
    public class Transcend : LowProfile.Behavior<Transcend.ModelType>
    {
        protected override bool IsEffective()
        {
            return Host.Owner.CardsOnBattlefield.Count == 1 && Host.Owner.CardsOnBattlefield[0] == Host;
        }

        [BehaviorModel(typeof(Transcend), Category = "v0.5/Passive", DefaultName = "超凡")]
        public class ModelType : LowProfile.ModelType
        {
            public ModelType()
                : base("atlas:Textures/Icons/Icons0$BTNInvisibility", "超凡\n该角色无法被攻击。")
            { }
        }
    }
}
