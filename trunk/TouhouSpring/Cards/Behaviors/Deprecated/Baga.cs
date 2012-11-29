using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TouhouSpring.Triggers;

namespace TouhouSpring.Behaviors
{
	public class Baga : BaseBehavior<Baga.ModelType>, ICastableSpell, ITrigger<Triggers.CardEnteredGraveyardContext>
	{
		public bool Cast(Game game, out string reason)
		{
			if (game.PlayerPlayer.Mana < Model.ManaCost)
			{
				reason = "Insufficient mana for casting ⑨.";
				return false;
			}

			var self = Host.Behaviors.Get<Warrior>();
			if (self.Attack <= 0)
			{
				reason = "已经笨到姥姥家了，无法施放⑨";
				return false;
			}

			var selected = new Interactions.SelectCards(
				game.PlayerController,
				game.OpponentPlayer.CardsOnBattlefield,
				Interactions.SelectCards.SelectMode.Single,
				"Select one enemy warrior to cast ⑨").Run();

			if (selected.Count == 0)
			{
				reason = "Casting is canceled.";
				return false;
			}
			LowWit lowwit = new LowWit(selected[0], Host);
			selected[0].Behaviors.Add(lowwit);
			game.UpdateMana(game.PlayerPlayer, -Model.ManaCost);

			reason = String.Empty;
			return true;
		}

		public void Trigger(CardEnteredGraveyardContext context)
		{
            if (context.Card != Host)
            {
                return;
            }
			foreach (var card in context.Game.PlayerPlayer.CardsOnBattlefield.Union(context.Game.OpponentPlayer.CardsOnBattlefield))
			{
				var lowwit = card.Behaviors.Get<LowWit>();
				if (lowwit != null && lowwit.FromCard == Host)
				{
					card.Behaviors.Remove(lowwit);
				}
			}
		}

        [BehaviorModel(typeof(Baga))]
        public class ModelType : BehaviorModel
        {
            public int ManaCost { get; set; }
        }
	}
}
