using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_FreeRedeem:
        BaseBehavior<Passive_FreeRedeem.ModelType>,
        Commands.ICause,
        IEpilogTrigger<Commands.MoveCard<Commands.Sacrifice, Commands.Hand>>
    {
        public void RunEpilog(Commands.MoveCard<Commands.Sacrifice, Commands.Hand> command)
        {
            if (Host.IsActivatedAssist)
            {
                Game.QueueCommands(new Commands.SummonMove<Commands.Sacrifice>(Host.Owner, Model.SummonType.Target));
            }
        }

        [BehaviorModel(typeof(Passive_FreeRedeem), Category = "v0.5/Passive", DefaultName = "毛玉替身")]
        public class ModelType : BehaviorModel
        {
            public CardModelReference SummonType { get; set; }
        }
    }
}
