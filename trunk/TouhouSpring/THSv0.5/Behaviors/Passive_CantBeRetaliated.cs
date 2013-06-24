using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_CantBeRetaliated : BaseBehavior<Passive_CantBeRetaliated.ModelType>,
        ILocalPrologTrigger<Commands.DealDamageToCard>
    {
        void ILocalPrologTrigger<Commands.DealDamageToCard>.RunLocalProlog(Commands.DealDamageToCard command)
        {
            if (command.Cause is Passive_Retaliate)
            {
                command.PatchDamageToDeal(0);
            }
        }

        [BehaviorModel(typeof(Passive_CantBeRetaliated), Category = "v0.5/Passive", DefaultName = "闪避", Description = "不受反击伤害")]
        public class ModelType : BehaviorModel
        { }
    }
}
