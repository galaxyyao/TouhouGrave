using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_AliceDollSummon : BaseBehavior<Passive_AliceDollSummon.ModelType>,
        IEpilogTrigger<Commands.StartTurn>
    {
        void IEpilogTrigger<Commands.StartTurn>.Run(Commands.StartTurn command)
        {
            if (command.Game.InPlayerPhases && IsOnBattlefield && command.Game.PlayerPlayer == Host.Owner)
            {
                1.Repeat(i =>
                {
                    command.Game.IssueCommands(new Commands.Summon(Model.SummonType.Target, Host.Owner));
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
