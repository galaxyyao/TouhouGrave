using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_Trample : BaseBehavior<Passive_Trample.ModelType>,
        Commands.ICause,
        IGlobalEpilogTrigger<Commands.DealDamageToCard>
    {
        void IGlobalEpilogTrigger<Commands.DealDamageToCard>.RunGlobalEpilog(Commands.DealDamageToCard command)
        {
            if (Host.Warrior != null && command.Cause == Host.Warrior)
            {
                var overflewDamage = Math.Max(-command.Target.Warrior.Life, 0);
                overflewDamage = Math.Min(overflewDamage, Host.Warrior.Attack);
                Game.QueueCommands(new Commands.SubtractPlayerLife(command.Target.Owner, overflewDamage, this));
            }
        }

        [BehaviorModel(typeof(Passive_Trample), Category = "v0.5/Passive", DefaultName = "践踏")]
        public class ModelType : BehaviorModel
        {
        }
    }
}
