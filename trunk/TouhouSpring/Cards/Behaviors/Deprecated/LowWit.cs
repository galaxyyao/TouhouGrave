using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
	public class LowWit : SimpleBehavior<LowWit>
	{
		public BaseCard FromCard
		{
			get; private set;
		}

		public BaseCard TargetCard
		{
			get; private set;
		}

		private readonly AttackModifier attackMod = new AttackModifier(x => x - 1);

		public LowWit(BaseCard targetCard, BaseCard fromCard)
		{
			TargetCard = targetCard;
			FromCard = fromCard;
		}

		protected override void OnBind()
		{
			var warrior = TargetCard.Behaviors.Get<Warrior>();
			if (warrior != null)
			{
				TargetCard.Behaviors.Add(attackMod);
				FromCard.Behaviors.Get<Warrior>().Attack.AddModifierToTail(x => x - 1);
			}
		}

		protected override void OnUnbind()
		{
			// TODO: better unbind if the Warrior behavior is unbound in advance
			var warrior = TargetCard.Behaviors.Get<Warrior>();
			if (warrior != null)
			{
				TargetCard.Behaviors.Remove(attackMod);
			}
		}
	}
}
