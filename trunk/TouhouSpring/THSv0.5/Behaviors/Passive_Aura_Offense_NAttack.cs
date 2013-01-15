using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_Aura_Offense_NAttack
    {

        [BehaviorModel(typeof(Passive_Aura_Offense_NAttack), DefaultName = "")]
        public class ModelType : BehaviorModel
        {
            public int AttackToMod
            {
                get;
                set;
            }
        }
    }
}
