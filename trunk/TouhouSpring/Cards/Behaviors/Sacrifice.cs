﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
	public class Sacrifice : BaseBehavior<Sacrifice.ModelType>,
        ITrigger<Triggers.PreCardPlayContext>, IPlayable
	{
		public void Trigger(Triggers.PreCardPlayContext context)
		{
			Game game = context.Game;

			if (!IsPlayable(game))
			{
				context.Cancel = true;
				context.Reason = "NoSacrifice";
				return;
			}

            IIndexable<BaseCard> selectCards = new Interactions.SelectCards(
                game.PlayerController,
                GetTargetSet().ToArray().ToIndexable(),
                Interactions.SelectCards.SelectMode.Single,
                String.Format("Select one warrior as the cost of summoning {0}.", Host.Model.Name)).Run();

			if (selectCards.Count == 0)
			{
				context.Cancel = true;
				context.Reason = "Sacrificing is canceled.";
			}
			else
			{
				game.DestroyCard(selectCards[0]);
			}
		}

		public bool IsPlayable(Game game)
		{
			return GetTargetSet().Any();
		}

        private IEnumerable<BaseCard> GetTargetSet()
        {
            return Host.Owner.CardsOnBattlefield.Where(card => card.Behaviors.Has<Warrior>() && card.State != CardState.CoolingDown);
        }

		[BehaviorModel("Sacrifice", typeof(Sacrifice))]
		public class ModelType : BehaviorModel
		{ }
	}
}
