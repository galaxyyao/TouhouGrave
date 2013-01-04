using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Immobilize : SimpleBehavior<Immobilize>,
        IEpilogTrigger<Commands.StartPhase>
    {
        void IEpilogTrigger<Commands.StartPhase>.Run(Commands.StartPhase command)
        {
            if (command.PhaseName == "Upkeep"
                && Game.ActingPlayer == Host.Owner
                && Host.IsOnBattlefield
                && Host.Behaviors.Has<Warrior>())
            {
                Game.IssueCommands(new Commands.SendBehaviorMessage(Host.Behaviors.Get<Warrior>(), "GoCoolingDown", null));
            }
        }
    }
}
