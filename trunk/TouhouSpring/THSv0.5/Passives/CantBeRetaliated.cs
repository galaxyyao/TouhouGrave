using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TouhouSpring.Behaviors;

namespace TouhouSpring.THSv0_5.Passives
{
    public sealed class CantBeRetaliated : BaseBehavior<CantBeRetaliated.ModelType>,
        ILocalPrologTrigger<Commands.DealDamageToCard>
    {
        void ILocalPrologTrigger<Commands.DealDamageToCard>.RunLocalProlog(Commands.DealDamageToCard command)
        {
            if (command.Cause is Retaliate)
            {
                command.PatchDamageToDeal(0);
            }
        }

        [BehaviorModel(typeof(CantBeRetaliated), Category = "v0.5/Passive", DefaultName = "闪避", Description = "不受反击伤害")]
        public class ModelType : BehaviorModel
        { }
    }
}
