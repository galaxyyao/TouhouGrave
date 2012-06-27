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
        public IIndexable<BaseCard> DeclaredAttackers
        {
            get;
            set;
        }

        internal BlockPhaseStartedContext(Game game, IIndexable<BaseCard> declaredAttackers)
            : base(game)
        {
            DeclaredAttackers = declaredAttackers;
        }
    }
}
