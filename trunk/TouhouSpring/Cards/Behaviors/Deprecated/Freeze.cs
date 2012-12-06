using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
	public class Freeze : BaseBehavior<Freeze.ModelType>, ICastableSpell
	{
		public class Effect : SimpleBehavior<Effect>
		{ }

		public bool Cast(Game game, out string reason)
		{
            IEnumerable<BaseCard> targetSet = game.OpponentPlayer.CardsOnBattlefield.Where(
                card => card.Behaviors.Has<Warrior>() && !card.Behaviors.Any(bhv => bhv is Effect));

			if (!targetSet.Any())
			{
				reason = "No card can be affected by this spell";
				return false;
			}

			if (Host.Owner.Mana < Model.ManaCost)
			{
				reason = "Insufficient mana";
				return false;
			}
			else if (Host.Behaviors.Get<Warrior>().State == WarriorState.CoolingDown)
			{
				reason = "Cannot cast under Cooling Down state";
				return false;
			}

			IIndexable<BaseCard> selectCards = new Interactions.SelectCards(
				game.PlayerController,
                targetSet.ToArray().ToIndexable(),
				Interactions.SelectCards.SelectMode.Single,
				"Select one enemy warrior to cast Freeze on.").Run();

			if (selectCards.Count == 0)
			{
				reason = "Casting is canceled.";
				return false;
			}

			var immoblize = new Immobilize();
			//var defenseModifier = new DefenseModifier(d => d - 1);
			var effect = new Effect();
			var lasting = new LastingEffect(1);
			lasting.CleanUps.Add(immoblize);
			lasting.CleanUps.Add(effect);

			var target = selectCards[0];

			game.SetWarriorState(target, WarriorState.CoolingDown);
			target.Behaviors.Add(immoblize);
			//target.Behaviors.Add(defenseModifier);
			target.Behaviors.Add(effect);
			target.Behaviors.Add(lasting);
			game.UpdateMana(Host.Owner, -Model.ManaCost);
            game.SetWarriorState(Host, WarriorState.CoolingDown);

			reason = String.Empty;
			return true;
		}

        [BehaviorModel(typeof(Freeze))]
        public class ModelType : BehaviorModel
        {
            public int ManaCost { get; set; }
        }
	}
}
