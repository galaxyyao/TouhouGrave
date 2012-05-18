using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Triggers
{
    /// <summary>
    /// Triggered before exiting the player's turn.
    /// </summary>
    public class PlayerTurnEndedContext : TriggerContext
    {
        internal PlayerTurnEndedContext(Game game)
            : base(game)
        { }
    }
}
