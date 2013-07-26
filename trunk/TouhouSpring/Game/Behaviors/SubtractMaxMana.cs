using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class SubtractMaxMana: BaseBehavior<SubtractMaxMana.ModelType>,
        Commands.ICause,
        IGlobalEpilogTrigger<Commands.PlayCard>
    {
        void IGlobalEpilogTrigger<Commands.PlayCard>.RunGlobalEpilog(Commands.PlayCard command)
        {
            if (command.Subject == Host)
                Game.QueueCommands(new Commands.SubtractPlayerMaxMana(Host.Owner, Model.ManaToSubtract, this));
        }

        [BehaviorModel(typeof(SubtractMaxMana), Category = "Core", DefaultName = "灵力池减小")]
        public class ModelType : BehaviorModel
        {
            public int ManaToSubtract
            {
                get;
                set;
            }
        }
    }
}
