using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TouhouSpring.Triggers;

namespace TouhouSpring.Behaviors
{
	public class ManaCost_PrePlay : BaseBehavior<ManaCost_PrePlay.ModelType>,
        ITrigger<PreCardPlayContext>, IPlayable
	{
		public void Trigger(PreCardPlayContext context)
		{
			if (context.CardToPlay == Host)
			{
				if (!IsPlayable(context.Game))
				{
					context.Cancel = true;
					context.Reason = "Insufficient mana";
				}
				else
				{
					context.Game.UpdateMana(Host.Owner, -Model.Cost);
				}
			}
		}

		public bool IsPlayable(Game game)
		{
			return Host.Owner.Mana >= Model.Cost;
		}

		[BehaviorModel("ManaCost (PrePlay)", typeof(ManaCost_PrePlay))]
		public class ModelType : BehaviorModel
		{
            public int Cost { get; set; }
		}
	}
}
