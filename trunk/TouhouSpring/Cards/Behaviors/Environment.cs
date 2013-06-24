using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Environment : BaseBehavior<Environment.ModelType>,
        Commands.ICause,
        IEpilogTrigger<Commands.IMoveCard>
    {
        public string VisualId
        {
            get { return Model.VisualId; }
        }

        public void RunEpilog(Commands.IMoveCard command)
        {
            if (command.Subject == Host
                && command.FromZoneType != ZoneType.OnBattlefield
                && command.ToZoneType == ZoneType.OnBattlefield)
            {
                foreach (var player in Game.Players)
                {
                    var lastEnv = player.CardsOnBattlefield.FirstOrDefault(
                        card => card != Host && card.Behaviors.Has<Environment>());
                    if (lastEnv != null)
                    {
                        Game.QueueCommands(new Commands.KillMove(lastEnv, this));
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
