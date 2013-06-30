using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_CardDrawnNumberUp :
        BaseBehavior<Passive_CardDrawnNumberUp.ModelType>,
        Commands.ICause,
        IGlobalEpilogTrigger<Commands.DrawMove>
    {
        public void RunGlobalEpilog(Commands.DrawMove command)
        {
            if (Host.IsOnBattlefield
                && command.Player == Host.Owner
                && command.Cause is Game)
            {
                Game.QueueCommands(new Commands.DrawMove(Host.Owner, SystemZone.Hand, this));
            }
        }

        [BehaviorModel(typeof(Passive_CardDrawnNumberUp), DefaultName = "抽卡增加", Category = "v0.5/Passive")]
        public class ModelType : BehaviorModel
        { }
    }
}
