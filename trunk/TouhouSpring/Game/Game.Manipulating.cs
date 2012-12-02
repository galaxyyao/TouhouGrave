using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using TouhouSpring.Interactions;

namespace TouhouSpring
{
	public partial class Game
	{
		public void CastSpell(Behaviors.ICastableSpell spell)
		{
			if (spell == null)
			{
				throw new ArgumentNullException("spell");
			}

			string reason = null;
			if (spell.Cast(this, out reason))
			{
				m_controllers.ForEach(c => c.InternalOnSpellCasted(spell));
			}
			else
			{
				m_controllers.ForEach(c => c.InternalOnSpellCastCanceled(spell, reason));
			}
		}

		/// <summary>
		/// Destroy a card.
		/// </summary>
		/// <param name="card">The card to be destroyed</param>
		public void DestroyCard(BaseCard card)
		{
			if (card == null)
			{
				throw new ArgumentNullException("card");
			}

			var preDestoryCtx = new Triggers.PreCardDestroyContext(this, card);
			if (!TriggerGlobal(preDestoryCtx))
			{
				// TODO: Card survives
				return;
			}

			var relevantCards = EnumerateRelevantCards().ToArray();

			if (card.Owner.m_battlefieldCards.Contains(card))
			{
				card.Owner.m_battlefieldCards.Remove(card);
				TriggerGlobal(new Triggers.CardLeftBattlefieldContext(this, card), relevantCards);
			}
			else if (card.Owner.m_handSet.Contains(card))
			{
				card.Owner.m_handSet.Remove(card);
			}
			else
			{
				throw new InvalidOperationException("Card can't be destroyed.");
			}

			// reset card states
			for (int i = 0; i < card.Behaviors.Count; ++i)
			{
				if (!card.Behaviors[i].Persistent)
				{
					card.Behaviors.RemoveAt(i);
					--i;
				}
			}

			if (card != card.Owner.Hero.Host)
			{
				card.Owner.m_graveyard.AddCardToTop(card);
				TriggerGlobal(new Triggers.CardEnteredGraveyardContext(this, card), relevantCards);
			}

			m_controllers.ForEach(c => c.InternalOnCardDestroyed(card));
		}

		public void TransferCard(BaseCard card, Player fromPlayer, Player toPlayer)
		{
			fromPlayer.m_battlefieldCards.Remove(card);
			toPlayer.m_battlefieldCards.Add(card);
			card.Owner = toPlayer;
		}
	}
}
