﻿using System;
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
		/// Deal health update to a player.
		/// </summary>
		/// <param name="player">The player whose health to be updated</param>
		/// <param name="delta">Amount of health delta</param>
		public void UpdateHealth(Player player, int delta, Behaviors.IBehavior cause)
		{
			if (player == null)
			{
				throw new ArgumentNullException("player");
			}
			else if (cause == null)
			{
				throw new ArgumentNullException("cause");
			}

			if (delta > 0)
			{
                player.Health += delta;
			}
			else if (delta < 0)
			{
				TriggerGlobal(new Triggers.PrePlayerDamageContext(this, player, -delta, cause));
				player.Health += delta;
				TriggerGlobal(new Triggers.PostPlayerDamagedContext(this, player, cause));
				m_controllers.ForEach(c => c.InternalOnPlayerDamaged(player, -delta));
			}
			else
			{
				// TODO: if health unchanged should be handled
			}
		}

		/// <summary>
		/// Deal mana update to a player.
		/// </summary>
		/// <param name="player">The player whose mana to be updated</param>
		/// <param name="delta">Amount of mana delta</param>
		/// <returns></returns>
		public bool UpdateMana(Player player, int delta)
		{
			if (player == null)
			{
				throw new ArgumentNullException("player");
			}

			int newMana = player.Mana + delta;
			if (newMana < 0)
			{
                new Interactions.MessageBox(PlayerController, "Not Enough Mana", MessageBox.Button.OK).Run();
				return false;
			}
			player.Mana = Math.Min(newMana, player.Hero.Model.Mana);
			return true;
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
			card.State = CardState.StandingBy;
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

		/// <summary>
		/// Draw a card for the specified player.
		/// </summary>
		/// <param name="player">The player to draw a card</param>
		public void DrawCard(Player player)
		{
			if (player == null)
			{
				throw new ArgumentNullException("player");
			}

			BaseCard card = player.m_library.RemoveCardFromTop();
			Debug.Assert(card != null && card.Owner == player);
			player.m_handSet.Add(card);
			m_controllers.ForEach(c => c.InternalOnCardDrawn(card));
            TriggerGlobal(new Triggers.PostCardDrawnContext(this, card));
		}

		/// <summary>
		/// Play a card.
		/// </summary>
		/// <param name="card">The card to be played</param>
		public void PlayCard(BaseCard card)
		{
			if (card == null)
			{
				throw new ArgumentNullException("card");
			}

			bool fromHand = card.Owner.m_handSet.Contains(card);

			////Temporarily no limitation on the number of warriors played in one turn.
			//if (fromHand && card.Behaviors.Has<Behaviors.Warrior>() && IsWarriorPlayedThisTurn)
			//{
			//    m_controllers.ForEach(c => c.InternalOnCardPlayCanceled(card, "Warrior has been played in this turn."));
			//    return;
			//}

			var prePlayCtx = new Triggers.PreCardPlayContext(this, card);
			if (TriggerGlobal(prePlayCtx, ctx => ctx.Cancel))
			{
				card.Owner.m_battlefieldCards.Add(card);
				if (fromHand)
				{
					card.Owner.m_handSet.Remove(card);
					IsWarriorPlayedThisTurn = IsWarriorPlayedThisTurn || card.Behaviors.Has<Behaviors.Warrior>();
				}
				m_controllers.ForEach(c => c.InternalOnCardPlayed(card));
                TriggerGlobal(new Triggers.PostCardPlayedContext(this, card));
			}
			else
			{
				m_controllers.ForEach(c => c.InternalOnCardPlayCanceled(card, prePlayCtx.Reason));
			}
		}

		public void TransferCard(BaseCard card, Player fromPlayer, Player toPlayer)
		{
			fromPlayer.m_battlefieldCards.Remove(card);
			toPlayer.m_battlefieldCards.Add(card);
			card.Owner = toPlayer;
		}

		/// <summary>
		/// Reset the cards' states in the specified player's battlefield.
		/// </summary>
		/// <param name="player">The player whose cards are going to be reset</param>
		public void ResetCardsStateInBattlefield(Player player)
		{
			if (player == null)
			{
				throw new ArgumentNullException("player");
			}

			player.m_battlefieldCards.ForEach(card => card.State = CardState.StandingBy);
		}

		public void SetCardState(BaseCard card, CardState state)
		{
			if (card == null)
			{
				throw new ArgumentNullException("card");
			}

			card.State = state;
		}

		/// <summary>
		/// Shuffle a player's library
		/// </summary>
		/// <param name="player">The player whose library is going to be shuffled</param>
		public void ShuffleLibrary(Player player)
		{
			if (player == null)
			{
				throw new ArgumentNullException("player");
			}

			player.m_library.Shuffle(m_randomGenerator);
		}
	}
}
