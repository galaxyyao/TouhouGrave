using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_Counterattack_NDamage:
        BaseBehavior<Passive_Counterattack_NDamage.ModelType>,
        Commands.ICause,
        IEpilogTrigger<Commands.DealDamageToCard>
    {
        void IEpilogTrigger<Commands.DealDamageToCard>.Run(Commands.DealDamageToCard command)
        {
            if (command.Target != Host)
                return;
            if (command.Cause is Warrior)
            {
                Game.IssueCommands(new Commands.DealDamageToCard(((Warrior)(command.Cause)).Host, Model.DamageToDeal, this));
            }
        }

        [BehaviorModel(typeof(Passive_Counterattack_NDamage), DefaultName = "反击")]
        public class ModelType : BehaviorModel
        {
            public int DamageToDeal
            {
                get;
                set;
            }
        }
    }
}
