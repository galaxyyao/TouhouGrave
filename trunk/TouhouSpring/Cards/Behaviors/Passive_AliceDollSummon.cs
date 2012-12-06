using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public class Passive_AliceDollSummon : BaseBehavior<Passive_AliceDollSummon.ModelType>,
        IEpilogTrigger<Commands.StartTurn>
    {
        void IEpilogTrigger<Commands.StartTurn>.Run(CommandContext<Commands.StartTurn> context)
        {
            if (context.Game.InPlayerPhases && IsOnBattlefield && context.Game.PlayerPlayer == Host.Owner)
            {
                1.Repeat(i =>
                {
                    context.Game.IssueCommands(new Commands.Summon
                    {
                        Model = Model.SummonType.Target,
                        Owner = Host.Owner
                    });
                });
            }
        }

        [BehaviorModel(typeof(Passive_AliceDollSummon), DefaultName = "人偶召唤")]
        public class ModelType : BehaviorModel
        {
            public CardModelReference SummonType { get; set; }
        }
    }
}
