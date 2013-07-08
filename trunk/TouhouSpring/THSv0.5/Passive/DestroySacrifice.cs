using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors.Passive
{
    public sealed class DestroySacrifice : BaseBehavior<DestroySacrifice.ModelType>,
        ILocalEpilogTrigger<Commands.PlayCard>,
        Commands.ICause
    {
        void ILocalEpilogTrigger<Commands.PlayCard>.RunLocalEpilog(Commands.PlayCard command)
        {
            if (Game.Players.Count == 2)
            {
                var opponent = Game.Players[1 - Host.Owner.Index];
                if (opponent.CardsSacrificed.Count > 0)
                {
                    var randomTarget = opponent.CardsSacrificed[Game.Random.Next(opponent.CardsSacrificed.Count)];
                    Game.QueueCommands(new Commands.MoveCard(randomTarget, SystemZone.Graveyard, this));
                }
            }
        }

        [BehaviorModel(typeof(DestroySacrifice), Category = "v0.5/Passive", DefaultName = "萧索")]
        public class ModelType : BehaviorModel
        { }
    }
}
