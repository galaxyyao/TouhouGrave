using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Assist_MassSputter_NDamage :
        BaseBehavior<Assist_MassSputter_NDamage.ModelType>,
        Commands.ICause,
        IEpilogTrigger<Commands.DealDamageToCard>,
        IEpilogTrigger<Commands.SubtractPlayerLife>
    {
        public void RunEpilog(Commands.DealDamageToCard command)
        {
            if (Host.IsActivatedAssist
                && command.Cause is Warrior
                && ((Warrior)command.Cause).Host.Owner == Host.Owner)
            {
                Game.QueueCommands(new Commands.SubtractPlayerLife(command.Target.Owner, Model.DamageToDeal, this));
            }
        }

        public void RunEpilog(Commands.SubtractPlayerLife command)
        {
            if (Host.IsActivatedAssist
                && command.FinalAmount > 0
                && command.Cause is Warrior
                && ((Warrior)command.Cause).Host.Owner == Host.Owner)
            {
                Game.QueueCommands(new Commands.SubtractPlayerLife(command.Player, Model.DamageToDeal, this));
            }
        }

        [BehaviorModel(typeof(Assist_MassSputter_NDamage), Category = "v0.5/Assist", DefaultName = "大规模溅射")]
        public class ModelType : BehaviorModel
        {
            public int DamageToDeal
            {
                get; set;
            }
        }
    }
}
