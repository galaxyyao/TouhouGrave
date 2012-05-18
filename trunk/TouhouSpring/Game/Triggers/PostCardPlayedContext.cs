using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Triggers
{
    /// <summary>
    /// Triggered after a card is played onto the player's battlefield.
    /// </summary>
    public class PostCardPlayedContext : TriggerContext
    {
        public BaseCard CardPlayed
        {
            get; private set;
        }

        internal PostCardPlayedContext(Game game, BaseCard cardPlayed) : base(game)
        {
            if (cardPlayed == null)
            {
                throw new ArgumentNullException("cardPlayed");
            }

            CardPlayed = cardPlayed;
        }
    }
}
