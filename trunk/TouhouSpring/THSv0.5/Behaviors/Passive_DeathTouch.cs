using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_DeathTouch :
        BaseBehavior<Passive_DeathTouch.ModelType>,
        Commands.ICause,
        IGlobalEpilogTrigger<Commands.DealDamageToCard>
    {
        public void RunGlobalEpilog(Commands.DealDamageToCard command)
        {
            if (command.Cause is Warrior
                && (command.Cause as Warrior).Host == Host)
            {
                Game.QueueCommands(new Commands.KillMove(command.Target, this));
            }
        }

        [BehaviorModel(typeof(Passive_DeathTouch), Category = "v0.5/Passive", DefaultName = "死神")]
        public class ModelType : BehaviorModel
        { }
    }
}
