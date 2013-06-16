using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_AliceDollSummon : BaseBehavior<Passive_AliceDollSummon.ModelType>,
        IEpilogTrigger<Commands.StartPhase>
    {
        public void RunEpilog(Commands.StartPhase command)
        {
            if (command.PhaseName == "Main"
                && Game.ActingPlayer == Host.Owner
                && (Host.IsOnBattlefield || Host.IsActivatedAssist))
            {
                1.Repeat(i =>
                {
                    Game.QueueCommands(new Commands.SummonMove<Commands.Battlefield>(Host.Owner, Model.SummonType.Target));
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
