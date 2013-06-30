using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_Retaliate : BaseBehavior<Passive_Retaliate.ModelType>,
        Commands.ICause,
        ILocalEpilogTrigger<Commands.DealDamageToCard>
    {
        public void RunLocalEpilog(Commands.DealDamageToCard command)
        {
            if (command.Cause is Warrior
                && Host.Warrior != null
                && command.Cause != Host.Warrior)
            {
                    Game.QueueCommands(new Commands.DealDamageToCard(
                        (command.Cause as Warrior).Host,
                        Host.Warrior.Attack,
                        this));
            }
        }

        [BehaviorModel(typeof(Passive_Retaliate), Category = "v0.5/Passive", DefaultName = "反击")]
        public class ModelType : BehaviorModel
        {
        }
    }
}
