using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_FakeSacrifice:
        BaseBehavior<Passive_FakeSacrifice.ModelType>,
        Commands.ICause,
        IEpilogTrigger<Commands.PlayCard>
    {
        void IEpilogTrigger<Commands.Redeem>.Run(Commands.Redeem command)
        {
            if (Game.ActingPlayer == Host.Owner
                && Host.Owner.ActivatedAssist==Host)
            {
                1.Repeat(i =>
                {
                    Game.IssueCommands(new Commands.Summon(Model.SummonType.Target, Host.Owner));
                });
            }
        }

        [BehaviorModel(typeof(Passive_FakeSacrifice), DefaultName = "")]
        public class ModelType : BehaviorModel
        {
            public CardModelReference SummonType { get; set; }
        }
    }
}
