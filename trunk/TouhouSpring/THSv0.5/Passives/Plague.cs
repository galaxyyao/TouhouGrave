using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TouhouSpring.Behaviors;

namespace TouhouSpring.THSv0_5.Passives
{
    public sealed class Plague : BaseBehavior<Plague.ModelType>,
        IGlobalEpilogTrigger<Commands.StartPhase>,
        Commands.ICause
    {
        void IGlobalEpilogTrigger<Commands.StartPhase>.RunGlobalEpilog(Commands.StartPhase command)
        {
            if (Game.ActingPlayer == Host.Owner
                && command.PhaseName == "Main")
            {
                var commands = new List<Commands.BaseCommand>(Game.Players.Sum(p => p.CardsOnBattlefield.Count));

                foreach (var card in Game.Players.SelectMany(p => p.CardsOnBattlefield))
                {
                    if (card != Host && card.Warrior != null)
                    {
                        commands.Add(new Commands.DealDamageToCard(card, Model.Damage, this));
                    }
                }

                Game.QueueCommands(commands.ToArray());
            }
        }

        [BehaviorModel(typeof(Plague), Category = "v0.5/Passive", DefaultName = "瘟疫")]
        public class ModelType : BehaviorModel
        {
            public int Damage { get; set; }
        }
    }
}
