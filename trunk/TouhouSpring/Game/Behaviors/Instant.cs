using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Instant : BaseBehavior<Instant.ModelType>,
        Commands.ICause,
        // TODO: Commands.MoveTo<Battlefield>
        IEpilogTrigger<Commands.MoveCard<Commands.Hand, Commands.Battlefield>>
    {
        public void RunEpilog(Commands.MoveCard<Commands.Hand, Commands.Battlefield> command)
        {
            if (command.Subject == Host)
            {
                Game.QueueCommands(new Commands.KillMove<Commands.Battlefield>(Host, this));
            }
        }

        [BehaviorModel(typeof(Instant), Category = "Core")]
        public class ModelType : BehaviorModel
        { }
    }
}
