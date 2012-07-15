using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public class Environment : BaseBehavior<Environment.ModelType>, ITrigger<Triggers.PostCardPlayedContext>
    {
        public void Trigger(Triggers.PostCardPlayedContext context)
        {
            if (context.CardPlayed == Host)
            {
                foreach (var player in context.Game.Players)
                {
                    var lastEnv = player.CardsOnBattlefield.FirstOrDefault(
                        card => card.Behaviors.Has<Environment>() && card != Host);
                    if (lastEnv != null)
                    {
                        context.Game.DestroyCard(lastEnv);
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
