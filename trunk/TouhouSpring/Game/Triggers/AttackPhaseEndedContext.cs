using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Triggers
{
    /// <summary>
    /// Triggered before player's attack phase ended.
    /// </summary>
    public class AttackPhaseEndedContext : TriggerContext
    {
        internal AttackPhaseEndedContext(Game game)
            : base(game)
        { }
    }
}
