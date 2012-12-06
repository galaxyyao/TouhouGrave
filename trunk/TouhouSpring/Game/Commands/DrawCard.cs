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
    public class DrawCard : ICommand
    {
        public string Token
        {
            get { return "DrawCard"; }
        }

        /// <summary>
        /// The player drawing a card
        /// </summary>
        public Player PlayerDrawing
        {
            get; set;
        }

        public BaseCard CardDrawn
        {
            get; private set;
        }

        public void Validate(Game game)
        {
            if (PlayerDrawing == null)
            {
                throw new CommandValidationFailException("PlayerDrawing can't be null.");
            }
            else if (!game.Players.Contains(PlayerDrawing))
            {
                throw new CommandValidationFailException("The Player object is not registered in game.");
            }
        }

        public void RunMain(Game game)
        {
            var card = PlayerDrawing.m_library.RemoveCardFromTop();
            Debug.Assert(card != null && card.Owner == PlayerDrawing);
            PlayerDrawing.m_handSet.Add(card);
            CardDrawn = card;
        }
    }
}
