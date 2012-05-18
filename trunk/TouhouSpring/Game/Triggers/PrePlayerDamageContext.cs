using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Triggers
{
    public class PrePlayerDamageContext : TriggerContext
    {
        public Player PlayerToDamage
        {
            get; private set;
        }

        public Behaviors.IBehavior Cause
        {
            get; private set;
        }

        public PrePlayerDamageContext(Game game, Player playerToDamage, int delta, Behaviors.IBehavior cause)
            : base(game)
        {
            if (playerToDamage == null)
            {
                throw new ArgumentNullException("playerToDamage");
            }
            else if (cause == null)
            {
                throw new ArgumentNullException("cause");
            }

            PlayerToDamage = playerToDamage;
            Cause = cause;
        }
    }
}
