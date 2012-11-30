using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TouhouSpring.Commands;
using TouhouSpring.Triggers;

namespace TouhouSpring.Behaviors
{
    public class SummonWeakness : BaseBehavior<SummonWeakness.ModelType>
        , ITrigger<PostCardPlayedContext>
        , IEpilogTrigger<EndTurn>
    {
        class Effect : SimpleBehavior<Effect>
        { }

        public void Trigger(PostCardPlayedContext context)
        {
            if (context.CardPlayed == Host)
            {
                Host.Behaviors.Add(new Effect());

                if (Host.Behaviors.Has<Warrior>())
                {
                    context.Game.IssueCommands(new SendBehaviorMessage
                    {
                        Target = Host.Behaviors.Get<Warrior>(),
                        Message = "GoCoolingDown"
                    });
                }
            }
        }

        void IEpilogTrigger<EndTurn>.Run(CommandContext<EndTurn> context)
        {
            if (IsOnBattlefield
                && context.Game.PlayerPlayer == Host.Owner
                && Host.Behaviors.Has<Effect>())
            {
                throw new NotImplementedException();
                // TODO: issue commands for the following:
                //Host.Behaviors.Remove(Host.Behaviors.Get<Effect>());
                if (Host.Behaviors.Has<Warrior>())
                {
                    context.Game.IssueCommands(new SendBehaviorMessage
                    {
                        Target = Host.Behaviors.Get<Warrior>(),
                        Message = "GoStandingBy"
                    });
                }
            }
        }

        [BehaviorModel(typeof(SummonWeakness))]
        public class ModelType : BehaviorModel
        { }
    }
}
