using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public class Instant : BaseBehavior<Instant.ModelType>,
        IEpilogTrigger<Commands.PlayCard>
    {
        void IEpilogTrigger<Commands.PlayCard>.Run(Commands.PlayCard command)
        {
            if (command.CardToPlay == Host)
            {
                command.Game.IssueCommands(new Commands.Kill(Host, this));
            }
        }

        [BehaviorModel(typeof(Instant), Category = "Core")]
        public class ModelType : BehaviorModel
        { }
    }
}
