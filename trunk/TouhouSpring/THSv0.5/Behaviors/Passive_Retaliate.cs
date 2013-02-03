using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_Retaliate : BaseBehavior<Passive_Retaliate.ModelType>,
        Commands.ICause,
        IEpilogTrigger<Commands.DealDamageToCard>
    {
        public void RunEpilog(Commands.DealDamageToCard command)
        {
            if (command.Target == Host && command.Cause is Warrior)
            {
                var warrior = Host.Behaviors.Get<Warrior>();
                if (warrior != null)
                {
                    Game.IssueCommands(new Commands.DealDamageToCard(
                        (command.Cause as Warrior).Host,
                        warrior.Attack,
                        this));
                }
            }
        }

        [BehaviorModel(Category = "v0.5/Passive", DefaultName = "反击")]
        public class ModelType : BehaviorModel<Passive_Retaliate>
        {
        }
    }
}
