using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Environment : BaseBehavior<Environment.ModelType>,
        Commands.ICause,
        IEpilogTrigger<Commands.MoveCard<Commands.Hand, Commands.Battlefield>>
    {
        public string VisualId
        {
            get { return Model.VisualId; }
        }

        public void RunEpilog(Commands.MoveCard<Commands.Hand, Commands.Battlefield> command)
        {
            if (command.Subject == Host)
            {
                foreach (var player in Game.Players)
                {
                    var lastEnv = player.CardsOnBattlefield.FirstOrDefault(
                        card => card != Host && card.Behaviors.Has<Environment>());
                    if (lastEnv != null)
                    {
                        Game.QueueCommands(new Commands.KillMove<Commands.Battlefield>(lastEnv, this));
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
