using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TouhouSpring.Commands;

namespace TouhouSpring.Behaviors
{
    public class Instant : BaseBehavior<Instant.ModelType>,
        IEpilogTrigger<PlayCard>
    {
        void IEpilogTrigger<PlayCard>.Run(CommandContext<PlayCard> context)
        {
            if (context.Command.CardToPlay == Host)
            {
                throw new NotImplementedException();
                // TODO: issue command for the following:
                //context.Game.DestroyCard(Host);
            }
        }

        [BehaviorModel(typeof(Instant), Category = "Core")]
        public class ModelType : BehaviorModel
        { }
    }
}
