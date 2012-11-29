using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TouhouSpring.Triggers;

namespace TouhouSpring.Behaviors
{
	public class Silence : SimpleBehavior<Silence>, ITrigger<PreCardPlayContext>
	{
		public void Trigger(PreCardPlayContext context)
		{
			BaseCard cardToPlay = context.CardToPlay;
			if (cardToPlay.Behaviors.Get<Warrior>() == null)
			{
				context.Cancel = true;
				context.Reason = "无法召唤魔法卡";
			}
		}
	}
}
