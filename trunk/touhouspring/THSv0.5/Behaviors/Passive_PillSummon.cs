using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_PillSummon:
        BaseBehavior<Passive_PillSummon.ModelType>,
        Commands.ICause,
        IEpilogTrigger<Commands.PlayCard>
    {
        public void RunEpilog(Commands.PlayCard command)
        {
            if (Game.ActingPlayer == Host.Owner)
            {
                Model.NumToSummon.Repeat(() =>
                {
                    Game.IssueCommands(new Commands.AddCardToManaPool(Model.SummonType.Target, Host.Owner));
                });
            }
        }

        [BehaviorModel(typeof(Passive_PillSummon), DefaultName = "毛玉召唤")]
        public class ModelType : BehaviorModel
        {
            public CardModelReference SummonType { get; set; }

            public int NumToSummon
            {
                get;
                set;
            }
        }
    }
}
