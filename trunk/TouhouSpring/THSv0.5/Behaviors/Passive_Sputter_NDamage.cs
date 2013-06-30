using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_Sputter_NDamage:
        BaseBehavior<Passive_Sputter_NDamage.ModelType>,
        Commands.ICause,
        IGlobalEpilogTrigger<Commands.DealDamageToCard>
    {
        public void RunGlobalEpilog(Commands.DealDamageToCard command)
        {
            if (Host.Warrior != null
                && command.Cause == Host.Warrior
                && Host.IsOnBattlefield)
            {
                Game.QueueCommands(new Commands.SubtractPlayerLife(command.Target.Owner, Model.DamageToDeal, this));
            }
        }

        [BehaviorModel(typeof(Passive_Sputter_NDamage), Category = "v0.5/Passive", DefaultName = "溅射")]
        public class ModelType : BehaviorModel
        {
            public int DamageToDeal
            {
                get; set;
            }
        }
    }
}
