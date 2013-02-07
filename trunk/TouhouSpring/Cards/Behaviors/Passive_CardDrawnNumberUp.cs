using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_CardDrawnNumberUp :
        BaseBehavior<Passive_CardDrawnNumberUp.ModelType>,
        IEpilogTrigger<Commands.DrawCard>
    {
        public void RunEpilog(Commands.DrawCard command)
        {
            if (Host.IsOnBattlefield)
            {
                Game.IssueCommands(new Commands.DrawCard(Host.Owner));
            }
        }

        [BehaviorModel(DefaultName = "星光")]
        public class ModelType : BehaviorModel<Passive_CardDrawnNumberUp>
        { }
    }
}
