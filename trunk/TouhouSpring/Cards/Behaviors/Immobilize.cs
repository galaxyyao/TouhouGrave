using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TouhouSpring.Commands;

namespace TouhouSpring.Behaviors
{
    public class Immobilize : SimpleBehavior<Immobilize>,
        IEpilogTrigger<StartTurn>
    {
        void IEpilogTrigger<StartTurn>.Run(CommandContext<StartTurn> context)
        {
            if (IsOnBattlefield && context.Game.PlayerPlayer == Host.Owner)
            {
                if (Host.Behaviors.Has<Warrior>())
                {
                    context.Game.IssueCommands(new SendBehaviorMessage
                    {
                        Target = Host.Behaviors.Get<Warrior>(),
                        Message = "GoCoolingDown"
                    });
                }
            }
        }
    }
}
