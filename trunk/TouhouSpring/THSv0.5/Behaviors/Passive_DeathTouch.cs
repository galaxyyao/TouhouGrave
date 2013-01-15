using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_DeathTouch :
        BaseBehavior<Passive_DeathTouch.ModelType>,
        Commands.ICause,
        IEpilogTrigger<Commands.DealDamageToCard>
    {
        void IEpilogTrigger<Commands.DealDamageToCard>.Run(Commands.DealDamageToCard command)
        {
            if (command.Cause is Warrior
                && (command.Cause as Warrior).Host == Host)
            {
                Game.IssueCommands(new Commands.Kill(command.Target, this));
            }
        }

        [BehaviorModel(typeof(Passive_DeathTouch), DefaultName = "死神")]
        public class ModelType : BehaviorModel
        { }
    }
}
