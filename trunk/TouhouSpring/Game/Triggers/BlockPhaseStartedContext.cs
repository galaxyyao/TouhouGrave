using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Triggers
{
    /// <summary>
    /// Triggered before player's block phase started.
    /// </summary>
    public class BlockPhaseStartedContext : TriggerContext
    {
        internal BlockPhaseStartedContext(Game game)
            : base(game)
        { }
    }
}
