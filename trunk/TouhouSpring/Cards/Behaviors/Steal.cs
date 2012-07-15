using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TouhouSpring.Triggers;

namespace TouhouSpring.Behaviors
{
	public class Steal : BaseBehavior<Steal.ModelType>, ICastableSpell, ITrigger<Triggers.PlayerTurnEndedContext>
	{
		public bool Cast(Game game, out string reason)
		{
			if (Host.Owner.Mana < Model.ManaCost)
			{
				reason = "Insufficient mana";
				return false;
			}

			if (game.OpponentPlayer.CardsOnBattlefield.Count == 0)
			{
				reason = "No card can be affected.";
				return false;
			}

			var selected = new Interactions.SelectCards(game.OpponentController,
			   game.OpponentPlayer.CardsOnBattlefield.Where(card => card.Behaviors.Has<Behaviors.Warrior>()).ToArray().ToIndexable(),
			   Interactions.SelectCards.SelectMode.Single,
			   "Select a warrior to steal.").Run();

			if (selected.Count == 0)
			{
				reason = "Casting is canceled.";
				return false;
			}

			BaseCard selectedCard = selected[0];
			CardTransfer cardTransfer = new CardTransfer(game, selectedCard, game.OpponentPlayer, game.PlayerPlayer);
			selectedCard.Behaviors.Add(cardTransfer);

			reason = string.Empty;
			return true;
		}

		public void Trigger(PlayerTurnEndedContext context)
		{	
			for (int i = 0; i < context.Game.PlayerPlayer.CardsOnBattlefield.Count; i++)
			{
				var cards = context.Game.PlayerPlayer.CardsOnBattlefield;
				if (cards[i].Behaviors.Get<Warrior>() == null)
					return;
				if (cards[i].Behaviors.Get<CardTransfer>() != null)
				{
					var cardTransfer = cards[i].Behaviors.Get<CardTransfer>();
					cards[i].Behaviors.Remove(cardTransfer);
					context.Game.TransferCard(cards[i], context.Game.PlayerPlayer, context.Game.OpponentPlayer);
					i--;
				}
			}
		}

        [BehaviorModel(typeof(Steal), DefaultName = "偷心")]
        public class ModelType : BehaviorModel
        {
            public int ManaCost { get; set; }
        }
	}
}
