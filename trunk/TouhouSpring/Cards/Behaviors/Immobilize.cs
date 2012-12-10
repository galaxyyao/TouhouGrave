using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public class Immobilize : SimpleBehavior<Immobilize>,
        IEpilogTrigger<Commands.StartTurn>
    {
        void IEpilogTrigger<Commands.StartTurn>.Run(Commands.StartTurn command)
        {
            if (IsOnBattlefield && command.Game.PlayerPlayer == Host.Owner)
            {
                if (Host.Behaviors.Has<Warrior>())
                {
                    command.Game.IssueCommands(new Commands.SendBehaviorMessage(Host.Behaviors.Get<Warrior>(), "GoCoolingDown", null));
                }
            }
        }
    }
}
