using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TouhouSpring.Commands
{
    /// <summary>
    /// Draw a card for the specified player.
    /// </summary>
    public class DrawCard : BaseCommand
    {
        public Player Player
        {
            get; private set;
        }

        public CardInstance CardDrawn
        {
            get; private set;
        }

        public DrawCard(Player player)
        {
            if (player == null)
            {
                throw new ArgumentNullException("player");
            }

            Player = player;
        }

        internal override void ValidateOnIssue()
        {
            Validate(Player);
        }

        internal override void ValidateOnRun()
        {
        }

        internal override void RunMain()
        {
            var cardModel = Player.Library.RemoveFromTop();
            Debug.Assert(cardModel != null);
            var card = new CardInstance(cardModel, Player);
            Player.AddToHandSorted(card);
            CardDrawn = card;
        }
    }
}
