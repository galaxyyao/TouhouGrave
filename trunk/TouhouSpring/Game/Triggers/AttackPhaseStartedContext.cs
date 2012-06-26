using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Triggers
{
    /// <summary>
    /// Triggered before player's attack phase started.
    /// </summary>
    public class AttackPhaseStartedContext : TriggerContext
    {
        internal AttackPhaseStartedContext(Game game)
            : base(game)
        { }
    }
}
