using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TouhouSpring.Commands
{
    /// <summary>
    /// Play a card from hand to battlefield.
    /// </summary>
    public class PlayCard : ICommand
    {
        public string Token
        {
            get { return "PlayCard"; }
        }

        /// <summary>
        /// The card to be played
        /// </summary>
        // TODO: change to serializable behavior ID
        public BaseCard CardToPlay
        {
            get; set;
        }

        public void Validate(Game game)
        {
            if (CardToPlay == null)
            {
                throw new CommandValidationFailException("CardToPlay can't be null.");
            }
            else if (!CardToPlay.Owner.CardsOnHand.Contains(CardToPlay))
            {
                throw new CommandValidationFailException("The card to play is not from player's hand.");
            }
            else if (!game.Players.Contains(CardToPlay.Owner))
            {
                throw new CommandValidationFailException("The owner of the card to be played is not registered in game.");
            }
        }

        public void RunMain(Game game)
        {
            CardToPlay.Owner.m_handSet.Remove(CardToPlay);
            CardToPlay.Owner.m_battlefieldCards.Add(CardToPlay);
        }
    }
}
