using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_FreeRedeem:
        BaseBehavior<Passive_FreeRedeem.ModelType>,
        Commands.ICause,
        IEpilogTrigger<Commands.Redeem>
    {
        public void RunEpilog(Commands.Redeem command)
        {
            if (Host.IsActivatedAssist)
            {
                Game.QueueCommands(new Commands.AddCardToManaPool(Model.SummonType.Target, Host.Owner));
            }
        }

        [BehaviorModel(typeof(Passive_FreeRedeem), Category = "v0.5/Passive", DefaultName = "毛玉替身")]
        public class ModelType : BehaviorModel
        {
            public CardModelReference SummonType { get; set; }
        }
    }
}
