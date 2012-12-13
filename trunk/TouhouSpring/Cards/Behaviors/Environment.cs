using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Environment : BaseBehavior<Environment.ModelType>,
        Commands.ICause,
        IEpilogTrigger<Commands.PlayCard>
    {
        void IEpilogTrigger<Commands.PlayCard>.Run(Commands.PlayCard command)
        {
            if (command.CardToPlay == Host)
            {
                foreach (var player in Game.Players)
                {
                    var lastEnv = player.CardsOnBattlefield.FirstOrDefault(
                        card => card != Host && card.Behaviors.Has<Environment>());
                    if (lastEnv != null)
                    {
                        Game.IssueCommands(new Commands.Kill(lastEnv, this));
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
