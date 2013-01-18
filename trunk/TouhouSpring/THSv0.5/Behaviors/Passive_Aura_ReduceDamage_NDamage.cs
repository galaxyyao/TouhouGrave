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
            //TODO: Modify if Cancel feature is available in Setup phase
            if (Host.Owner.ActivatedAssist != Host)
                return;
            if (command.Cause is Warrior)
            {
                Game.IssueCommands(new Commands.HealCard(command.Target, Model.DamageReduced, this));
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
