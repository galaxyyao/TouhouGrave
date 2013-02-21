using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_Bleed_ToEnemiesNDamageEachTurn
        : BaseBehavior<Passive_Bleed_ToEnemiesNDamageEachTurn.ModelType>,
        Commands.ICause,
        IEpilogTrigger<Commands.StartTurn>
    {
        public void RunEpilog(Commands.StartTurn command)
        {
            if (Game.ActingPlayer == Host.Owner)
            {
                foreach (var player in Game.ActingPlayerEnemies)
                {
                    Game.IssueCommands(new Commands.SubtractPlayerLife(player, Model.DamageToDeal, this));
                }
            }
        }

        [BehaviorModel(Category = "v0.5/Passive", DefaultName = "对敌方玩家回合伤害")]
        public class ModelType : BehaviorModel<Passive_Bleed_ToEnemiesNDamageEachTurn>
        {
            public int DamageToDeal
            {
                get;
                set;
            }
        }
    }
}
