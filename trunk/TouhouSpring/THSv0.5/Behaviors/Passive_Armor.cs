using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_Armor:BaseBehavior<Passive_Armor.ModelType>,
        ILocalPrologTrigger<Commands.DealDamageToCard>,
        Commands.ICause
    {
        void ILocalPrologTrigger<Commands.DealDamageToCard>.RunLocalProlog(Commands.DealDamageToCard command)
        {
            if(command.Cause is Warrior
                || command.Cause is Retaliate)
                command.PatchDamageToDeal(Math.Max(command.DamageToDeal - Model.DamageToMod, 0));
        }

        [BehaviorModel(typeof(Passive_Armor), Category = "v0.5/Passive", DefaultName = "护甲")]
        public class ModelType : BehaviorModel
        {
            public int DamageToMod
            {
                get;
                set;
            }
        }
    }
}
