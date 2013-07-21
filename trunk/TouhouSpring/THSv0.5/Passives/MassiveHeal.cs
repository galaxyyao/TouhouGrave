using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TouhouSpring.Behaviors;

namespace TouhouSpring.THSv0_5.Passives
{
    public sealed class MassiveHeal : BaseBehavior<MassiveHeal.ModelType>,
        Commands.ICause,
        IGlobalEpilogTrigger<Commands.EndPhase>
    {
        void IGlobalEpilogTrigger<Commands.EndPhase>.RunGlobalEpilog(Commands.EndPhase command)
        {
            if (command.PreviousPhase == "Main"
                && Game.ActingPlayer == Host.Owner)
            {
                foreach (var card in Host.Owner.CardsOnBattlefield)
                {
                    if (card == Host && !Model.HealSelf)
                    {
                        continue;
                    }

                    Game.QueueCommands(new Commands.HealCard(card, Model.Amount, this));
                }
            }
        }

        [BehaviorModel(typeof(MassiveHeal), Category = "v0.5/Passive", DefaultName = "治疗（回合结束时）")]
        public class ModelType : BehaviorModel
        {
            public int Amount { get; set; }
            public bool HealSelf { get; set; }
        }
    }
}
