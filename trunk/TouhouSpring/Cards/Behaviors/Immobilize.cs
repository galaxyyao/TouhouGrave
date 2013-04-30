﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Immobilize : SimpleBehavior<Immobilize>,
        IEpilogTrigger<Commands.StartPhase>
    {
        public void RunEpilog(Commands.StartPhase command)
        {
            if (command.PhaseName == "Upkeep"
                && Game.ActingPlayer == Host.Owner
                && Host.IsOnBattlefield
                && Host.Behaviors.Has<Warrior>())
            {
                Game.QueueCommands(new Commands.SendBehaviorMessage(Host.Behaviors.Get<Warrior>(), "GoCoolingDown", null));
            }
        }
    }
}
