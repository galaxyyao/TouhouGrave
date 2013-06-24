using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_Shield_NDamage:
        BaseBehavior<Passive_Shield_NDamage.ModelType>,
        Commands.ICause,
        ILocalEpilogTrigger<Commands.DealDamageToCard>
    {
        public void RunLocalEpilog(Commands.DealDamageToCard command)
        {
            Game.QueueCommands(new Commands.HealCard(Host, Model.DamageToMod, this));
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
