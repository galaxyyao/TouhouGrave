using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Triggers
{
    /// <summary>
    /// Triggered after entering the player's turn.
    /// </summary>
    public class PlayerTurnStartedContext : TriggerContext
    {
        internal PlayerTurnStartedContext(Game game) : base(game)
        { }
    }
}
