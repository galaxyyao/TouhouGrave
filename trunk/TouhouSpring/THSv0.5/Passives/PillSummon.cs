using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TouhouSpring.Behaviors;

namespace TouhouSpring.THSv0_5.Passives
{
    public sealed class PillSummon : BaseBehavior<PillSummon.ModelType>,
        Commands.ICause,
        ILocalEpilogTrigger<Commands.PlayCard>
    {
        void ILocalEpilogTrigger<Commands.PlayCard>.RunLocalEpilog(Commands.PlayCard command)
        {
            Model.NumToSummon.Repeat(() =>
            {
                Game.QueueCommands(new Commands.SummonMove(Model.SummonType.Value, Host.Owner, SystemZone.Sacrifice));
            });
        }

        [BehaviorModel(typeof(PillSummon), Category = "v0.5/Passive", DefaultName = "毛玉召唤")]
        public class ModelType : BehaviorModel
        {
            public CardModelReference SummonType { get; set; }

            public int NumToSummon
            {
                get; set;
            }
        }
    }
}
