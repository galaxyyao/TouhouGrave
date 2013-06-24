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
            // TODO: cache hostwarrior
            var hostWarrior = Host.Behaviors.Get<Warrior>();
            if (hostWarrior != null && command.Cause == hostWarrior)
            {
                var overflewDamage = Math.Max(-command.Target.Behaviors.Get<Warrior>().Life, 0);
                Game.QueueCommands(new Commands.SubtractPlayerLife(command.Target.Owner, Math.Min(overflewDamage, hostWarrior.Attack), this));
            }
        }

        [BehaviorModel(typeof(Passive_Trample), Category = "v0.5/Passive", DefaultName = "践踏")]
        public class ModelType : BehaviorModel
        {
        }
    }
}
