using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_WarriorAlwaysStandBy :
        BaseBehavior<Passive_WarriorAlwaysStandBy.ModelType>,
        IEpilogTrigger<Commands.StartPhase>
    {
        public void RunEpilog(Commands.StartPhase command)
        {
            if (command.PhaseName == "Cleanup"
                && Host.IsOnBattlefield && Host.Behaviors.Has<Warrior>())
            {
                Game.IssueCommands(new Commands.SendBehaviorMessage(Host.Behaviors.Get<Warrior>(), "GoStandingBy", null));
            }
        }

        [BehaviorModel]
        public class ModelType : BehaviorModel<Passive_WarriorAlwaysStandBy>
        { }
    }
}
