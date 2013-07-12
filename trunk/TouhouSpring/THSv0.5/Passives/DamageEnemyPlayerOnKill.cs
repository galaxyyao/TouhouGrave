using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TouhouSpring.Behaviors;

namespace TouhouSpring.THSv0_5.Passives
{
    public sealed class DamageEnemyPlayerOnKill : BaseBehavior<DamageEnemyPlayerOnKill.ModelType>,
        ILocalEpilogTrigger<Commands.IMoveCard>,
        Commands.ICause
    {
        void ILocalEpilogTrigger<Commands.IMoveCard>.RunLocalEpilog(Commands.IMoveCard command)
        {
            if (command.ToZone == SystemZone.Graveyard)
            {
                foreach (var player in Game.Players)
                {
                    if (player != Host.Owner)
                    {
                        Game.QueueCommands(new Commands.SubtractPlayerLife(player, Model.Damage, this));
                    }
                }
            }
        }

        [BehaviorModel(typeof(DamageEnemyPlayerOnKill), Category = "v0.5/Passive", DefaultName = "怨念")]
        public class ModelType : BehaviorModel
        {
            public int Damage { get; set; }
        }
    }
}
