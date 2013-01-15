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
        void IEpilogTrigger<Commands.StartTurn>.Run(Commands.StartTurn command)
        {
            if (Game.ActingPlayer == Host.Owner)
            {
                foreach (var player in Game.ActingPlayerEnemies)
                {
                    Game.IssueCommands(new Commands.DealDamageToPlayer(player, Model.DamageToDeal, this));
                }
            }
        }

        [BehaviorModel(typeof(Passive_Bleed_ToEnemiesNDamageEachTurn), DefaultName = "对敌方玩家回合伤害")]
        public class ModelType : BehaviorModel
        {
            public int DamageToDeal
            {
                get;
                set;
            }
        }
    }
}
