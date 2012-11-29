using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TouhouSpring.Triggers;

namespace TouhouSpring.Behaviors
{
    public class SummonWeakness : BaseBehavior<SummonWeakness.ModelType>
        , ITrigger<PostCardPlayedContext>
        , Commands.IEpilogTrigger<Commands.EndTurn>
    {
        class Effect : SimpleBehavior<Effect>
        { }

        public void Trigger(PostCardPlayedContext context)
        {
            if (context.CardPlayed == Host)
            {
                Host.Behaviors.Add(new Effect());
                context.Game.SetWarriorState(Host, WarriorState.CoolingDown);
            }
        }

        void Commands.IEpilogTrigger<Commands.EndTurn>.Run(Commands.CommandContext context)
        {
            if (IsOnBattlefield
                && context.Game.PlayerPlayer == Host.Owner
                && Host.Behaviors.Has<Effect>())
            {
                throw new NotImplementedException();
                // TODO: issue commands for the following:
                //Host.Behaviors.Remove(Host.Behaviors.Get<Effect>());
                //context.Game.SetWarriorState(Host, WarriorState.StandingBy);
            }
        }

        [BehaviorModel(typeof(SummonWeakness))]
        public class ModelType : BehaviorModel
        { }
    }
}
