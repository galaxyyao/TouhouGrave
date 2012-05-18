using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Triggers
{
    /// <summary>
    /// Triggered after a card is destroyed.
    /// </summary>
    public class PostCardDestroyedContext : TriggerContext
    {
        public BaseCard CardDestroyed
        {
            get; private set;
        }

        internal PostCardDestroyedContext(Game game, BaseCard cardDestroyed)
            : base(game)
        {
            if (cardDestroyed == null)
            {
                throw new ArgumentNullException("cardDestroyed");
            }

            CardDestroyed = cardDestroyed;
        }
    }
}
