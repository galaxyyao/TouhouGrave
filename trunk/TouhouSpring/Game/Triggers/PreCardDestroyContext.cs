using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Triggers
{
    /// <summary>
    /// Triggered before a card is destroyed.
    /// </summary>
    public class PreCardDestroyContext : CancelableTriggerContext
    {
        public BaseCard CardToDestroy
        {
            get; private set;
        }

        internal PreCardDestroyContext(Game game, BaseCard cardToDestroy)
            : base(game)
        {
            if (cardToDestroy == null)
            {
                throw new ArgumentNullException("cardToDestroy");
            }

            CardToDestroy = cardToDestroy;
        }
    }
}
