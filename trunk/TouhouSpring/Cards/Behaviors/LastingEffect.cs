using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
	public class LastingEffect : SimpleBehavior<LastingEffect>, ITrigger<Triggers.PlayerTurnStartedContext>
	{
		public int Duration
		{
			get; private set;
		}

		/// <summary>
		/// Other effect that will be removed together with lastingeffect
		/// </summary>
		public List<IBehavior> CleanUps
		{
			get; private set;
		}

		public LastingEffect(int duration)
		{
			if (duration <= 0)
			{
				throw new ArgumentOutOfRangeException("Duration should be a positive integer.");
			}
			Duration = duration;
            CleanUps = new List<IBehavior>();
		}

		public void Trigger(Triggers.PlayerTurnStartedContext context)
		{
			if (IsOnBattlefield && context.Game.PlayerPlayer == Host.Owner && --Duration == 0)
			{
				CleanUps.ForEach(bhv => Host.Behaviors.Remove(bhv));
			}
		}
	}
}
