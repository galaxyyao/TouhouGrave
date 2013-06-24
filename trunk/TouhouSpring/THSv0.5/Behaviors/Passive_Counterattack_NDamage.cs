using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_Counterattack_NDamage:
        BaseBehavior<Passive_Counterattack_NDamage.ModelType>,
        Commands.ICause,
        ILocalEpilogTrigger<Commands.DealDamageToCard>
    {
        public void RunLocalEpilog(Commands.DealDamageToCard command)
        {
            var warrior = command.Cause as Warrior;
            if (warrior != null && warrior.Host.Owner != Host.Owner)
            {
                Game.QueueCommands(new Commands.DealDamageToCard(warrior.Host, Model.DamageToDeal, this));
            }
        }

        [BehaviorModel(typeof(Passive_Counterattack_NDamage), Category = "v0.5/Passive", DefaultName = "反击")]
        public class ModelType : BehaviorModel
        {
            public int DamageToDeal
            {
                get; set;
            }
        }
    }
}
