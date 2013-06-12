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
    public class PlayCard : BaseCommand, IInitiativeCommand
    {
        // TODO: change to serializable behavior ID
        public CardInstance CardToPlay
        {
            get; private set;
        }

        public Player Initiator
        {
            get { return CardToPlay.Owner; }
        }

        public PlayCard(CardInstance cardToPlay)
        {
            if (cardToPlay == null)
            {
                throw new ArgumentNullException("cardToPlay");
            }

            CardToPlay = cardToPlay;
        }

        internal override void ValidateOnIssue()
        {
            Validate(CardToPlay);
            if (CardToPlay.IsHero)
            {
                if (CardToPlay.Owner.CardsOnBattlefield.Contains(CardToPlay))
                {
                    FailValidation("The hero card is already on the battlefield.");
                }
            }
            else if (!CardToPlay.Owner.CardsOnHand.Contains(CardToPlay))
            {
                FailValidation("The card to play is not from player's hand.");
            }
        }

        internal override bool ValidateOnRun()
        {
            return !CardToPlay.IsDestroyed && CardToPlay.Owner.CardsOnHand.Contains(CardToPlay);
        }

        internal override void RunMain()
        {
            CardToPlay.Owner.m_handSet.Remove(CardToPlay);
            CardToPlay.Owner.m_battlefieldCards.Add(CardToPlay);
            Context.Game.SubscribeCardToCommands(CardToPlay);
        }
    }
}
