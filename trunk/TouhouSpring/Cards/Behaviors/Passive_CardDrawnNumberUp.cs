using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_CardDrawnNumberUp :
        BaseBehavior<Passive_CardDrawnNumberUp.ModelType>,
        Commands.ICause,
        IEpilogTrigger<Commands.DrawMove>
    {
        public void RunEpilog(Commands.DrawMove command)
        {
            if (command.Cause is Game && Host.IsOnBattlefield)
            {
                Game.QueueCommands(new Commands.DrawMove(Host.Owner, SystemZone.Hand, this));
            }
        }

        [BehaviorModel(typeof(Passive_CardDrawnNumberUp), DefaultName = "星光")]
        public class ModelType : BehaviorModel
        { }
    }
}
