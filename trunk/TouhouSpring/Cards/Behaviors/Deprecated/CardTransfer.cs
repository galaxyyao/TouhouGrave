using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
	public class CardTransfer : SimpleBehavior<CardTransfer>
	{
		public CardTransfer(Game game, BaseCard card, Player fromPlayer, Player toPlayer)
		{
			game.TransferCard(card, fromPlayer, toPlayer);
		}
	}
}
