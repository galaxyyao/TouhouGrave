using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_WarriorAlwaysStandBy :
        BaseBehavior<Passive_WarriorAlwaysStandBy.ModelType>,
        IGlobalEpilogTrigger<Commands.StartPhase>
    {
        public void RunGlobalEpilog(Commands.StartPhase command)
        {
            if (command.PhaseName == "Cleanup"
                && Host.IsOnBattlefield && Host.Behaviors.Has<Warrior>())
            {
                Game.QueueCommands(new Commands.SendBehaviorMessage(Host.Behaviors.Get<Warrior>(), "GoStandingBy", null));
            }
        }

        [BehaviorModel(typeof(Passive_WarriorAlwaysStandBy))]
        public class ModelType : BehaviorModel
        { }
    }
}
