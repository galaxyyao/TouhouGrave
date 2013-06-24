using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Instant : BaseBehavior<Instant.ModelType>,
        Commands.ICause,
        ILocalEpilogTrigger<Commands.PlayCard>
    {
        public void RunLocalEpilog(Commands.PlayCard command)
        {
            if (command.ToZone == SystemZone.Battlefield)
            {
                Game.QueueCommands(new Commands.KillMove(Host, this));
            }
        }

        [BehaviorModel(typeof(Instant), Category = "Core")]
        public class ModelType : BehaviorModel
        { }
    }
}
