using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Immobilize : SimpleBehavior<Immobilize>,
        IEpilogTrigger<Commands.StartTurn>
    {
        void IEpilogTrigger<Commands.StartTurn>.Run(Commands.StartTurn command)
        {
            if (IsOnBattlefield && Game.ActingPlayer == Host.Owner)
            {
                if (Host.Behaviors.Has<Warrior>())
                {
                    Game.IssueCommands(new Commands.SendBehaviorMessage(Host.Behaviors.Get<Warrior>(), "GoCoolingDown", null));
                }
            }
        }
    }
}
