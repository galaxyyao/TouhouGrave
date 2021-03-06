﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TouhouSpring.Behaviors;

namespace TouhouSpring.THSv0_5.Passives
{
    public class Transcend : Utilities.LowProfile.Behavior<Transcend.ModelType>
    {
        protected override bool IsEffective()
        {
            return Host.Owner.CardsOnBattlefield.Count == 1 && Host.Owner.CardsOnBattlefield[0] == Host;
        }

        [BehaviorModel(typeof(Transcend), Category = "v0.5/Passive", DefaultName = "超凡")]
        public class ModelType : Utilities.LowProfile.ModelType
        {
            public ModelType()
                : base("atlas:Textures/Icons/Icons0$BTNInvisibility", "超凡\n该角色无法被攻击。")
            { }
        }
    }
}
