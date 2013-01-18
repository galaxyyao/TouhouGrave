using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_DrawCardOnPlay:
        BaseBehavior<Passive_DrawCardOnPlay.ModelType>,
        Commands.ICause,
        IEpilogTrigger<Commands.PlayCard>
    {
        public void RunEpilog(Commands.PlayCard command)
        {
            if (command.CardToPlay == Host)
                Game.IssueCommands(new Commands.DrawCard(Host.Owner));
        }

        [BehaviorModel(typeof(Passive_DrawCardOnPlay), DefaultName = "上场抽卡")]
        public class ModelType : BehaviorModel
        {
        }
    }
}
