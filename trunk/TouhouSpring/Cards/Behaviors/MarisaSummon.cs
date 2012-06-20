using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
	public class MarisaSummon : BaseBehavior<MarisaSummon.ModelType>,
		IPlayable,
		ITrigger<Triggers.CardEnteredGraveyardContext>
	{
		public bool IsPlayable(Game game)
		{
			return true;
		}

		public void Trigger(Triggers.CardEnteredGraveyardContext context)
		{
			
		}

		[BehaviorModel("Marisa Behavior", typeof(MarisaSummon))]
		public class ModelType : BehaviorModel
		{ }
	}
}
