using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_SpiritForm : BaseBehavior<Passive_SpiritForm.ModelType>,
        ILocalPrologTrigger<Commands.DealDamageToCard>,
        ILocalPrologTrigger<Commands.HealCard>
    {
        void ILocalPrologTrigger<Commands.DealDamageToCard>.RunLocalProlog(Commands.DealDamageToCard command)
        {
            command.PatchDamageToDeal(Math.Max(Math.Min(command.DamageToDeal, 1), 0));
        }

        void ILocalPrologTrigger<Commands.HealCard>.RunLocalProlog(Commands.HealCard command)
        {
            command.PatchLifeToHeal(0);
        }

        [BehaviorModel(typeof(Passive_SpiritForm), Category = "v0.5/Passive", DefaultName = "灵体")]
        public class ModelType : BehaviorModel
        {
        }
    }
}
