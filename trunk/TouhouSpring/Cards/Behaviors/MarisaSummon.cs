using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
	public class MarisaSummon : BaseBehavior<MarisaSummon.ModelType>,
		ITrigger<Triggers.PreCardPlayContext>, IPlayable,
		ITrigger<Triggers.CardEnteredGraveyardContext>,
		ITrigger<Triggers.CardLeftBattlefieldContext>,
		ITrigger<Triggers.PlayerTurnStartedContext>,
		ITrigger<Triggers.PostCardPlayedContext>
	{
		private int m_summonCounter = 0;
		private int m_lastingCounter = 0;
        private readonly Func<int, int> m_auraAttackMod = x => x + 1;

		public void Trigger(Triggers.PreCardPlayContext context)
		{
			if (context.CardToPlay == Host)
			{
				if (!IsPlayable(context.Game))
				{
					context.Cancel = true;
					context.Reason = "Marisa can't be summoned yet.";
					return;
				}
				{
					m_lastingCounter = 3;
				}
			}
		}

		public bool IsPlayable(Game game)
		{
			return m_summonCounter >= 1;
		}

		public void Trigger(Triggers.CardEnteredGraveyardContext context)
		{
			if (context.Card.Owner == Host.Owner && context.Card.Behaviors.Has<Warrior>())
			{
				++m_summonCounter;
			}
		}

		public void Trigger(Triggers.CardLeftBattlefieldContext context)
		{
			if (context.Card == Host)
			{
				m_summonCounter = 0;
                context.Game.PlayerPlayer.ManaDelta--;
                context.Card.Behaviors.Get<Warrior>().Attack.RemoveModifier(m_auraAttackMod);
                foreach (var card in context.Game.PlayerPlayer.CardsOnBattlefield)
                {
                    if (card.Behaviors.Get<Warrior>() == null)
                    {
                        continue;
                    }
                    card.Behaviors.Get<Warrior>().Attack.RemoveModifier(m_auraAttackMod);
                }
			}
		}

		public void Trigger(Triggers.PlayerTurnStartedContext context)
		{
			if (IsOnBattlefield && context.Game.PlayerPlayer == Host.Owner)
			{
				if (--m_lastingCounter == 0)
				{
					context.Game.DestroyCard(Host);
				}
			}
		}

		public void Trigger(Triggers.PostCardPlayedContext context)
		{
			if(context.CardPlayed==Host)
			{
				context.Game.PlayerPlayer.ManaDelta++;
				foreach (var card in context.Game.PlayerPlayer.CardsOnBattlefield)
				{
                    if (card.Behaviors.Get<Warrior>() == null)
                        continue;
                    card.Behaviors.Get<Warrior>().Attack.AddModifierToTail(m_auraAttackMod);
				}
				return;
			}
            if (context.CardPlayed.Owner == Host.Owner
                && context.CardPlayed.Behaviors.Get<Warrior>() != null
                && Host.Owner.CardsOnBattlefield.Contains(Host))
            {
                context.CardPlayed.Behaviors.Get<Warrior>().Attack.AddModifierToTail(m_auraAttackMod);
            }
		}

		[BehaviorModel("Marisa Behavior", typeof(MarisaSummon))]
		public class ModelType : BehaviorModel
		{ }
	}
}
