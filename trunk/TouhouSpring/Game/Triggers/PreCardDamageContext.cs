using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Triggers
{
    /// <summary>
    /// Triggered before a card is damaged.
    /// </summary>
    public class PreCardDamageContext : CancelableTriggerContext
    {
        public BaseCard CardToDamage
        {
            get; private set;
        }

    	public int DamageToDeal
    	{
    		get; set;
    	}

        public Behaviors.IBehavior Cause
        {
            get; private set;
        }

        internal PreCardDamageContext(Game game, BaseCard cardToDamage, int damage, Behaviors.IBehavior cause)
            : base(game)
        {
            if (cardToDamage == null)
            {
                throw new ArgumentNullException("cardToDamage");
            }
            else if (cause == null)
            {
                throw new ArgumentNullException("cause");
            }

            CardToDamage = cardToDamage;
        	DamageToDeal = damage;
            Cause = cause;
        }
    }
}
