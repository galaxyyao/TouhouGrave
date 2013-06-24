using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_DrawCardOnPlay:
        BaseBehavior<Passive_DrawCardOnPlay.ModelType>,
        Commands.ICause,
        ILocalEpilogTrigger<Commands.PlayCard>
    {
        public void RunLocalEpilog(Commands.PlayCard command)
        {
            Game.QueueCommands(new Commands.DrawMove(Host.Owner, SystemZone.Hand));
        }

        [BehaviorModel(typeof(Passive_DrawCardOnPlay), Category = "v0.5/Passive", DefaultName = "上场抽卡")]
        public class ModelType : BehaviorModel
        {
        }
    }
}
