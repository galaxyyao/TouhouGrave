using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TouhouSpring.Triggers;

namespace TouhouSpring.Behaviors
{
    public class SummonWeakness : BaseBehavior<SummonWeakness.ModelType>
        , ITrigger<PostCardPlayedContext>
        , ITrigger<PlayerTurnEndedContext>
    {
        public void Trigger(PostCardPlayedContext context)
        {
            if(!IsOnBattlefield)
                return;
            if (context.CardPlayed.Behaviors.Get<Warrior>() != null
                && context.CardPlayed.Owner == Host.Owner)
            {
                context.CardPlayed.Behaviors.Add(new Weakness(context.CardPlayed));
            }
        }

        public void Trigger(PlayerTurnEndedContext context)
        {
            if (!IsOnBattlefield)
                return;
            if (context.Game.PlayerPlayer == Host.Owner && Host.Behaviors.Get<Weakness>()!=null)
            {
                var weakness = Host.Behaviors.Get<Weakness>();
                Host.Behaviors.Remove(weakness);
            }
        }

        [BehaviorModel("Summon Weakness", typeof(SummonWeakness))]
        public class ModelType : BehaviorModel
        { }
    }
}
