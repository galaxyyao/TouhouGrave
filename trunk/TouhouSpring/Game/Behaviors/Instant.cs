using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public class Instant : BaseBehavior<Instant.ModelType>, ITrigger<Triggers.PostCardPlayedContext>
    {
        public void Trigger(Triggers.PostCardPlayedContext context)
        {
            if (context.CardPlayed == Host)
            {
                context.Game.DestroyCard(Host);
            }
        }

        [BehaviorModel("Instant", typeof(Instant))]
        public class ModelType : BehaviorModel
        { }
    }
}
