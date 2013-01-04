using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_DamageWhenWarriorDestroyed:
        BaseBehavior<Passive_DamageWhenWarriorDestroyed.ModelType>,
        Commands.ICause,
        IEpilogTrigger<Commands.Kill>
    {
        void IEpilogTrigger<Commands.Kill>.Run(Commands.Kill command)
        {
            if (command.Target == Host && command.EnteredGraveyard)
            {
                if (!(command.Cause is Warrior))
                    return;
                Game.IssueCommands(new Commands.DealDamageToCard((command.Cause as Warrior).Host, Model.Damage, this));
            }
        }


        [BehaviorModel(typeof(Passive_DamageWhenWarriorDestroyed), DefaultName = "死后反击")]
        public class ModelType : BehaviorModel
        {
            public int Damage { get; set; }
        }
    }
}
