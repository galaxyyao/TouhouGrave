using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TouhouSpring.Commands;
using TouhouSpring.Triggers;

namespace TouhouSpring.Behaviors
{
    public class SummonWeakness : BaseBehavior<SummonWeakness.ModelType>
        , IEpilogTrigger<PlayCard>
        , IEpilogTrigger<EndTurn>
    {
        class Effect : SimpleBehavior<Effect>
        { }

        void IEpilogTrigger<PlayCard>.Run(CommandContext<PlayCard> context)
        {
            if (context.Command.CardToPlay == Host
                && Host.Behaviors.Has<Warrior>())
            {
                context.Game.IssueCommands(
                    new AddBehavior
                    {
                        Target = Host,
                        Behavior = new Effect()
                    },
                    new SendBehaviorMessage
                    {
                        Target = Host.Behaviors.Get<Warrior>(),
                        Message = "GoCoolingDown"
                    }
                );
            }
        }

        void IEpilogTrigger<EndTurn>.Run(CommandContext<EndTurn> context)
        {
            if (IsOnBattlefield
                && context.Game.PlayerPlayer == Host.Owner
                && Host.Behaviors.Has<Warrior>()
                && Host.Behaviors.Has<Effect>())
            {
                context.Game.IssueCommands(
                    new RemoveBehavior
                    {
                        Target = Host,
                        Behavior = Host.Behaviors.Get<Effect>()
                    },
                    new SendBehaviorMessage
                    {
                        Target = Host.Behaviors.Get<Warrior>(),
                        Message = "GoStandingBy"
                    }
                );
            }
        }

        [BehaviorModel(typeof(SummonWeakness))]
        public class ModelType : BehaviorModel
        { }
    }
}
