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
				context.Game.SetCardState(Host, CardState.CoolingDown);
			}
		}
	}
}
