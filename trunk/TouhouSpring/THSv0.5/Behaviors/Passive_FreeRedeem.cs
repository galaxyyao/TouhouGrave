using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_FreeRedeem:
        BaseBehavior<Passive_FreeRedeem.ModelType>,
        Commands.ICause,
        IGlobalEpilogTrigger<Commands.IMoveCard>
    {
        public void RunGlobalEpilog(Commands.IMoveCard command)
        {
            if (command.FromZone == SystemZone.Sacrifice
                && Host.IsActivatedAssist)
            {
                Game.QueueCommands(new Commands.SummonMove(Model.SummonType.Target, Host.Owner, SystemZone.Sacrifice));
            }
        }

        [BehaviorModel(typeof(Passive_FreeRedeem), Category = "v0.5/Passive", DefaultName = "毛玉替身")]
        public class ModelType : BehaviorModel
        {
            public CardModelReference SummonType { get; set; }
        }
    }
}
