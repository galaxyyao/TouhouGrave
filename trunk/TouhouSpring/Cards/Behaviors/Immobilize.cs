using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Immobilize : SimpleBehavior<Immobilize>,
        IGlobalEpilogTrigger<Commands.StartPhase>
    {
        public void RunGlobalEpilog(Commands.StartPhase command)
        {
            if (command.PhaseName == "Upkeep"
                && Game.ActingPlayer == Host.Owner
                && Host.Warrior != null
                && Host.IsOnBattlefield)
            {
                Game.QueueCommands(new Commands.SendBehaviorMessage(Host.Warrior, WarriorMessage.GoCoolingDown, null));
            }
        }
    }
}
