using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TouhouSpring.Commands;

namespace TouhouSpring.Behaviors
{
    public class Environment : BaseBehavior<Environment.ModelType>,
        IEpilogTrigger<PlayCard>
    {
        void IEpilogTrigger<PlayCard>.Run(CommandContext<PlayCard> context)
        {
            if (context.Command.CardToPlay == Host)
            {
                foreach (var player in context.Game.Players)
                {
                    var lastEnv = player.CardsOnBattlefield.FirstOrDefault(
                        card => card.Behaviors.Has<Environment>() && card != Host);
                    if (lastEnv != null)
                    {
                        throw new NotImplementedException();
                        // TODO: issue command for the following:
                        //context.Game.DestroyCard(lastEnv);
                        break;
                    }
                }
            }
        }

        [BehaviorModel(typeof(Environment))]
        public class ModelType : BehaviorModel
        {
            // TODO: (BHV) Select VisualID from textures
            public string VisualId { get; set; }
        }
    }
}
