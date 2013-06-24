using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Instant : BaseBehavior<Instant.ModelType>,
        Commands.ICause,
        IEpilogTrigger<Commands.PlayCard>
    {
        public void RunEpilog(Commands.PlayCard command)
        {
            if (command.ToZone == SystemZone.Battlefield
                && command.Subject == Host)
            {
                Game.QueueCommands(new Commands.KillMove(Host, this));
            }
        }

        [BehaviorModel(typeof(Instant), Category = "Core")]
        public class ModelType : BehaviorModel
        { }
    }
}
