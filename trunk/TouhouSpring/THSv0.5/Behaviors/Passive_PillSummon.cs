using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_PillSummon:
        BaseBehavior<Passive_PillSummon.ModelType>,
        Commands.ICause,
        IEpilogTrigger<Commands.MoveCard<Commands.Hand, Commands.Battlefield>>
    {
        public void RunEpilog(Commands.MoveCard<Commands.Hand, Commands.Battlefield> command)
        {
            if (Game.ActingPlayer == Host.Owner)
            {
                Model.NumToSummon.Repeat(() =>
                {
                    Game.QueueCommands(new Commands.SummonMove<Commands.Sacrifice>(Host.Owner, Model.SummonType.Target));
                });
            }
        }

        [BehaviorModel(typeof(Passive_PillSummon), Category = "v0.5/Passive", DefaultName = "毛玉召唤")]
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
