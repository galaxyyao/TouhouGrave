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
            if (Host.Owner.ActivatedAssist == Host)
            {
                Game.IssueCommands(new Commands.AddCardToManaPool(Model.SummonType.Target, Host.Owner));
            }
        }

        [BehaviorModel(Category = "v0.5/Passive", DefaultName = "毛玉替身")]
        public class ModelType : BehaviorModel<Passive_FreeRedeem>
        {
            public CardModelReference SummonType { get; set; }
        }
    }
}
