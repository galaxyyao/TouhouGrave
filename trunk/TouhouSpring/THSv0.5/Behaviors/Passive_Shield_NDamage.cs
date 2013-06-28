using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_Shield_NDamage: BaseBehavior<Passive_Shield_NDamage.ModelType>,
        ILocalPrologTrigger<Commands.DealDamageToCard>,
        Commands.ICause
    {
        void ILocalPrologTrigger<Commands.DealDamageToCard>.RunLocalProlog(Commands.DealDamageToCard command)
        {
            command.PatchDamageToDeal(Math.Max(command.DamageToDeal - Model.DamageToMod, 0));
        }

        [BehaviorModel(typeof(Passive_Shield_NDamage), Category = "v0.5/Passive", DefaultName = "厚皮")]
        public class ModelType : BehaviorModel
        {
            public int DamageToMod
            {
                get; set;
            }
        }
    }
}
