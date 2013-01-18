using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_RegenerateFull :
        BaseBehavior<Passive_RegenerateFull.ModelType>,
        Commands.ICause,
        IEpilogTrigger<Commands.StartTurn>
    {
        public void RunEpilog(Commands.StartTurn command)
        {
            if (Host.Owner == Game.ActingPlayer
                && Host.IsOnBattlefield)
                Game.IssueCommands(new Commands.HealCard(Host
                    , Host.Behaviors.Get<Warrior>().MaxLife - Host.Behaviors.Get<Warrior>().Life
                    , this));
        }

        [BehaviorModel(typeof(Passive_RegenerateFull), DefaultName = "单卡完全恢复")]
        public class ModelType : BehaviorModel
        {
        }
    }
}
