using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TouhouSpring.Behaviors;

namespace TouhouSpring.THSv0_5.Assists
{
    public sealed class SummonOnActivate : BaseBehavior<SummonOnActivate.ModelType>,
        ILocalEpilogTrigger<Commands.ActivateAssist>
    {
        void ILocalEpilogTrigger<Commands.ActivateAssist>.RunLocalEpilog(Commands.ActivateAssist command)
        {
            Game.QueueCommands(new Commands.SummonMove(Model.SummonType.Value, Host.Owner, SystemZone.Battlefield));
        }

        [BehaviorModel(typeof(SummonOnActivate), Category = "v0.5/Assist", DefaultName = "人形制作")]
        public class ModelType : BehaviorModel
        {
            public CardModelReference SummonType { get; set; }
        }
    }
}
