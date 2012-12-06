using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public class SummonWeakness : BaseBehavior<SummonWeakness.ModelType>
        , IEpilogTrigger<Commands.PlayCard>
        , IEpilogTrigger<Commands.EndTurn>
    {
        class Effect : SimpleBehavior<Effect>
        { }

        void IEpilogTrigger<Commands.PlayCard>.Run(CommandContext<Commands.PlayCard> context)
        {
            if (context.Command.CardToPlay == Host
                && Host.Behaviors.Has<Warrior>())
            {
                context.Game.IssueCommands(
                    new Commands.AddBehavior
                    {
                        Target = Host,
                        Behavior = new Effect()
                    },
                    new Commands.SendBehaviorMessage
                    {
                        Target = Host.Behaviors.Get<Warrior>(),
                        Message = "GoCoolingDown"
                    }
                );
            }
        }

        void IEpilogTrigger<Commands.EndTurn>.Run(CommandContext<Commands.EndTurn> context)
        {
            if (IsOnBattlefield
                && context.Game.PlayerPlayer == Host.Owner
                && Host.Behaviors.Has<Warrior>()
                && Host.Behaviors.Has<Effect>())
            {
                context.Game.IssueCommands(
                    new Commands.RemoveBehavior
                    {
                        Target = Host,
                        Behavior = Host.Behaviors.Get<Effect>()
                    },
                    new Commands.SendBehaviorMessage
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
