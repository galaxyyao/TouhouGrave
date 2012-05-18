using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
	public class Weakness : SimpleBehavior<Weakness>
	{
		public BaseCard TargetCard
		{
			get; private set;
		}

		public Weakness(BaseCard card)
		{
			TargetCard = card;
		}

		protected override void OnBind()
		{
			TargetCard.State = CardState.CoolingDown;
		}

		protected override void OnUnbind()
		{
			TargetCard.State = CardState.StandingBy;
		}
	}
}
