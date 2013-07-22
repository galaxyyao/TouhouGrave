using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TouhouSpring.Behaviors;

namespace TouhouSpring.THSv0_5.Utilities
{
    public static class SuppressHeal
    {
        public class Behavior<T> : BaseBehavior<T>,
            ILocalPrologTrigger<Commands.HealCard>
            where T : ModelType
        {
            void ILocalPrologTrigger<Commands.HealCard>.RunLocalProlog(Commands.HealCard command)
            {
                if (Model.Amount <= 0)
                {
                    command.PatchLifeToHeal(0);
                }
                else
                {
                    command.PatchLifeToHeal(Math.Max(command.LifeToHeal - Model.Amount, 0));
                }
            }
        }

        public class ModelType : BehaviorModel
        {
            // 0 means to suppress all healings
            public int Amount { get; set; }
        }
    }
}
