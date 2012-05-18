using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Triggers
{
    /// <summary>
    /// Triggered after a card is damaged.
    /// </summary>
	public class PostCardDamagedContext : CancelableTriggerContext
    {
        public BaseCard CardDamaged
        {
            get; private set;
        }

        public int DamageDealt
        {
            get; private set;
        }

        public Behaviors.IBehavior Cause
        {
            get; private set;
        }

        internal PostCardDamagedContext(Game game, BaseCard cardDamaged, int damageDealt, Behaviors.IBehavior cause)
            : base(game)
        {
            if (cardDamaged == null)
            {
                throw new ArgumentNullException("cardDamaged");
            }

            CardDamaged = cardDamaged;
            DamageDealt = damageDealt;
            Cause = cause;
        }
    }
}
