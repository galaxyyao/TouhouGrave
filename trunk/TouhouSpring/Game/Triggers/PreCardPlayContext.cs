using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Triggers
{
    /// <summary>
    /// Triggered before a card is played onto the player's battlefield.
    /// </summary>
    public class PreCardPlayContext : CancelableTriggerContext
    {
        public BaseCard CardToPlay
        {
            get; private set;
        }

        internal PreCardPlayContext(Game game, BaseCard cardToPlay)
            : base(game)
        {
            if (cardToPlay == null)
            {
                throw new ArgumentNullException("cardToPlay");
            }

            CardToPlay = cardToPlay;
        }
    }
}
