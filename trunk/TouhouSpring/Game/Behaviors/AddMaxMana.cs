using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class AddMaxMana : BaseBehavior<AddMaxMana.ModelType>,
        Commands.ICause,
        IGlobalEpilogTrigger<Commands.PlayCard>
    {
        void IGlobalEpilogTrigger<Commands.PlayCard>.RunGlobalEpilog(Commands.PlayCard command)
        {
            if (command.Subject == Host)
                Game.QueueCommands(new Commands.AddPlayerMaxMana(Host.Owner, Model.ManaToAdd, this));
        }

        [BehaviorModel(typeof(AddMaxMana), Category = "Core", DefaultName = "灵力池扩大")]
        public class ModelType : BehaviorModel
        {
            public int ManaToAdd
            {
                get;
                set;
            }
        }
    }


}
