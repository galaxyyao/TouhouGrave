using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Triggers
{
    public class PostPlayerDamagedContext : TriggerContext
    {
        public Player PlayerDamaged
        {
            get; private set;
        }

        public Behaviors.IBehavior Cause
        {
            get; private set;
        }

        public PostPlayerDamagedContext(Game game, Player playerDamaged, Behaviors.IBehavior cause)
            : base(game)
        {
            if (playerDamaged == null)
            {
                throw new ArgumentNullException("playerDamaged");
            }
            else if (cause == null)
            {
                throw new ArgumentNullException("cause");
            }

            PlayerDamaged = playerDamaged;
            Cause = cause;
        }
    }
}
