using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public class Environment : BaseBehavior<Environment.ModelType>,
        IEpilogTrigger<Commands.PlayCard>
    {
        void IEpilogTrigger<Commands.PlayCard>.Run(CommandContext<Commands.PlayCard> context)
        {
            if (context.Command.CardToPlay == Host)
            {
                foreach (var player in context.Game.Players)
                {
                    var lastEnv = player.CardsOnBattlefield.FirstOrDefault(
                        card => card.Behaviors.Has<Environment>() && card != Host);
                    if (lastEnv != null)
                    {
                        context.Game.IssueCommands(new Commands.Kill
                        {
                            Target = lastEnv,
                            Cause = this
                        });
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
