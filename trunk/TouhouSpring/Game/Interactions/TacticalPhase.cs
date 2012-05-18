using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Interactions
{
	public class TacticalPhase : SelectCards
	{
		public TacticalPhase(BaseController controller)
			: base(controller, ComputeFromSet(controller).ToArray().ToIndexable(), SelectMode.Single,
				   "Select a card from hand to play onto the battlefield or cast a spell from a card on battlefield.")
		{
			if (controller == null)
			{
				throw new ArgumentNullException("controller");
			}
			else if (controller != controller.Game.PlayerController)
			{
				throw new InvalidOperationException("TacticalPhase can only be invoked on the current player.");
			}
		}

		public new object Run()
		{
			var result = NotifyAndWait<object>(Controller);
			Validate(result);
			return result;
		}

		public override void Respond(IIndexable<BaseCard> selectedCards)
		{
			Validate(selectedCards);
			RespondBack(Controller, selectedCards == null || selectedCards.Count == 0 ? null : selectedCards[0]);
		}

		public void Respond(BaseCard selectedCard)
		{
			Validate(selectedCard);
			RespondBack(Controller, selectedCard);
		}

		public void Respond(Behaviors.ICastableSpell selectedSpell)
		{
			Validate(selectedSpell);
			RespondBack(Controller, selectedSpell);
		}

		public IIndexable<BaseCard> ComputeCastFromSet()
		{
			return Controller.Player.CardsOnBattlefield.Where(card=>card.State==CardState.StandingBy).ToArray().ToIndexable();
		}

		protected void Validate(object selected)
		{
			if (selected == null)
			{
				return;
			}

			if (selected is IIndexable<BaseCard>)
			{
				base.Validate((IIndexable<BaseCard>)selected);
			}
			else if (selected is BaseCard)
			{
				base.Validate(new BaseCard[] { (BaseCard)selected }.ToIndexable());
			}
            else if (selected is Behaviors.ICastableSpell)
            {
				if (!ComputeCastFromSet().Contains(((Behaviors.ICastableSpell)selected).Host))
				{
					throw new InvalidDataException("Selected spell doesn't come from a card from player's battlefield.");
				}
            }
			else
			{
				throw new InvalidDataException("Selected object is neither a card nor a spell.");
			}
		}

        private static IEnumerable<BaseCard> ComputeFromSet(BaseController controller)
        {
            return GetFromSet(controller.Player).Where(card => !card.Behaviors.OfType<Behaviors.IPlayable>().Any(p => !p.IsPlayable(controller.Game)));
        }

        private static IEnumerable<BaseCard> GetFromSet(Player player)
        {
            foreach (var card in player.CardsOnHand)
            {
                yield return card;
            }

            if (!player.CardsOnBattlefield.Contains(player.Hero.Host))
            {
                // hero card is on battlefield
                yield return player.Hero.Host;
            }
        }
	}
}
