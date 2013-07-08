using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors.Passive
{
    public class BeGuarded : LowProfile.Behavior<BeGuarded.ModelType>
    {
        protected override bool IsEffective()
        {
            return Host.Owner.CardsOnBattlefield.Any(card => card.Model == Model.BeGuardedBy.Value);
        }

        [BehaviorModel(typeof(BeGuarded), Category = "v0.5/Passive", DefaultName = "花隐")]
        public class ModelType : LowProfile.ModelType
        {
            public CardModelReference BeGuardedBy
            {
                get; set;
            }

            public ModelType()
                : base("atlas:Textures/Icons/Icons0$BTNInvisibility", "花隐\n在向日葵的护卫下该角色无法被攻击。")
            { }
        }
    }
}
