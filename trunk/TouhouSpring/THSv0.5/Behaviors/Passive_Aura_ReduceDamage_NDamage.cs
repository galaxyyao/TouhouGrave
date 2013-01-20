using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_Aura_ReduceDamage_NDamage :
        BaseBehavior<Passive_Aura_ReduceDamage_NDamage.ModelType>,
        Commands.ICause,
        IPrologTrigger<Commands.DealDamageToCard>
    {
        public void RunProlog(Commands.DealDamageToCard command)
        {
            if (Host.Owner.ActivatedAssist == Host)
            {
                if (command.Target.Owner == Host.Owner
                    && command.Cause is Warrior)
                {
                    command.PatchDamageToDeal(Math.Max(command.DamageToDeal - Model.DamageReduced, 0));
                }
            }
        }

        [BehaviorModel(typeof(Passive_Aura_ReduceDamage_NDamage), DefaultName = "减伤光环")]
        public class ModelType : BehaviorModel
        {
            public int DamageReduced
            {
                get;
                set;
            }
        }
    }
}
