using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_CardDrawnNumberUp :
        BaseBehavior<Passive_CardDrawnNumberUp.ModelType>,
        Commands.ICause,
        IEpilogTrigger<Commands.DrawMove<Commands.Hand>>
    {
        public void RunEpilog(Commands.DrawMove<Commands.Hand> command)
        {
            if (command.Cause is Game && Host.IsOnBattlefield)
            {
                Game.QueueCommands(new Commands.DrawMove<Commands.Hand>(Host.Owner, this));
            }
        }

        [BehaviorModel(typeof(Passive_CardDrawnNumberUp), DefaultName = "星光")]
        public class ModelType : BehaviorModel
        { }
    }
}
