using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
	public class Immobilize : SimpleBehavior<Immobilize>, ITrigger<Triggers.PlayerTurnStartedContext>
	{
		public void Trigger(Triggers.PlayerTurnStartedContext context)
		{
			if (IsOnBattlefield && context.Game.PlayerPlayer == Host.Owner)
			{
                if (Host.Behaviors.Has<Warrior>())
                {
                    context.Game.IssueCommands(new Commands.SendBehaviorMessage
                    {
                        Target = Host.Behaviors.Get<Warrior>(),
                        Message = "GoCoolingDown"
                    });
                }
			}
		}
	}
}
