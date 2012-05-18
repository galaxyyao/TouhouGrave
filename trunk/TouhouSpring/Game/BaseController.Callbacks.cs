using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
	public partial class BaseController
	{
		/// <summary>
		/// Game calls this method when a card is destroyed.
		/// </summary>
		/// <param name="card"></param>
		internal void InternalOnCardDestroyed(BaseCard card)
		{
			Debug.Assert(card != null && card.Owner != null);

			if (card.Owner == Player)
			{
				new Interactions.NotifyCardEvent(this, "OnCardDestroyed", card).Run();
			}
			else
			{
				;
			}
		}

		/// <summary>
		/// Game calls this method when a card is drawn.
		/// </summary>
		/// <param name="card">The card drawn</param>
		internal void InternalOnCardDrawn(BaseCard card)
		{
			Debug.Assert(card != null && card.Owner != null);

			if (card.Owner == Player)
			{
				new Interactions.NotifyCardEvent(this, "OnCardDrawn", card).Run();
			}
			else
			{
				;
			}
		}

		/// <summary>
		/// Game calls this method when a card is played.
		/// </summary>
		/// <param name="card">The card played</param>
		internal void InternalOnCardPlayed(BaseCard card)
		{
			Debug.Assert(card != null && card.Owner != null);

			if (card.Owner == Player)
			{
				new Interactions.NotifyCardEvent(this, "OnCardPlayed", card).Run();
			}
			else
			{
			}
		}

		/// <summary>
		/// Game calls this method when a card play is canceled.
		/// </summary>
		/// <param name="card">The card played</param>
		/// <param name="reason">The reason why the card play canceled</param>
		internal void InternalOnCardPlayCanceled(BaseCard card, string reason)
		{
			Debug.Assert(card != null && card.Owner != null);
			Debug.Assert(reason != null);

			if (card.Owner == Player)
			{
				new Interactions.NotifyCardEvent((this), "OnCardPlayCanceled", card, reason).Run();
			}
			else
			{
				;
			}
		}

		/// <summary>
		/// Game calls this method when damages are dealt to player.
		/// </summary>
		/// <param name="player">Player who receives damage</param>
		/// <param name="damage">The amount of damage being dealt</param>
        internal void InternalOnPlayerDamaged(Player player, int damage)
		{
			Debug.Assert(player != null);

			if (player == Player)
			{
				new Interactions.NotifyControllerEvent(this, "OnPlayerDamaged", player, string.Format("Damage:{0}", damage)).Run();
			}
			else
			{
			}
		}

		/// <summary>
		/// Game calls this method when the phase is changed among two of the five player phases.
		/// </summary>
		/// <param name="player">Whose turn</param>
		internal void InternalOnPlayerPhaseChanged(Player player)
		{
			Debug.Assert(player != null);

			if (player == Player)
			{
				new Interactions.NotifyControllerEvent(this, "OnPlayerPhaseChanged", player, Game.CurrentPhase).Run();
			}
			else
			{
			}
		}

		internal void InternalOnSpellCasted(Behaviors.ICastableSpell spell)
		{
			Debug.Assert(spell != null);

			if (spell.Host.Owner == Player)
			{
				new Interactions.NotifySpellEvent(this, "OnSpellCasted", spell).Run();
			}
		}

		internal void InternalOnSpellCastCanceled(Behaviors.ICastableSpell spell, string reason)
		{
			Debug.Assert(spell != null);

			if (spell.Host.Owner == Player)
			{
				new Interactions.NotifySpellEvent(this, "OnSpellCastCanceled", spell, reason).Run();
			}
		}
	}
}
