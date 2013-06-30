using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_HealSpring :
        BaseBehavior<Passive_HealSpring.ModelType>,
        Commands.ICause,
        IGlobalEpilogTrigger<Commands.EndTurn>
    {
        public void RunGlobalEpilog(Commands.EndTurn command)
        {
            if (Host.IsOnBattlefield
                && Game.ActingPlayer == Host.Owner)
            {
                foreach (var card in Game.ActingPlayer.CardsOnBattlefield)
                {
                    if (card == Host)
                        continue;
                    Game.QueueCommands(new Commands.HealCard(card, Model.LifeToHeal, this));
                }
            }
        }

        [BehaviorModel(typeof(Passive_HealSpring), Category = "v0.5/Passive", DefaultName = "治疗泉")]
        public class ModelType : BehaviorModel
        {
            public int LifeToHeal
            {
                get;
                set;
            }
        }
    }
}
