using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Triggers
{
    /// <summary>
    /// Triggered before player's block phase ended.
    /// </summary>
    public class BlockPhaseEndedContext : TriggerContext
    {
        internal BlockPhaseEndedContext(Game game)
            : base(game)
        { }
    }
}
