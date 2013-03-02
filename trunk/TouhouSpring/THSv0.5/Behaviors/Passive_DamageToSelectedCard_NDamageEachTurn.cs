using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_DamageToSelectedCard_NDamageEachTurn :
        BaseBehavior<Passive_DamageToSelectedCard_NDamageEachTurn.ModelType>,
        Commands.ICause,
        IEpilogTrigger<Commands.EndPhase>
    {
        public void RunEpilog(Commands.EndPhase command)
        {
            if (command.PreviousPhase == "Upkeep"
                && Game.ActingPlayer == Host.Owner)
            {
                var targetCandidates = Game.ActingPlayerEnemies
                    .SelectMany(p => p.CardsOnBattlefield)
                    .Where(c => c.Behaviors.Has<Warrior>())
                    .ToArray();
                if (targetCandidates.Length > 0)
                {
                    var selectedCard = new Interactions.SelectCards(
                        Host.Owner,
                        targetCandidates.ToIndexable(),
                        Interactions.SelectCards.SelectMode.Single,
                        "指定1张对手的卡，造成伤害").Run();

                    if (selectedCard.Count > 0)
                    {
                        Game.IssueCommands(new Commands.DealDamageToCard(selectedCard[0], Model.DamageToDeal, this));
                    }
                }
            }
        }

        [BehaviorModel(typeof(Passive_DamageToSelectedCard_NDamageEachTurn), Category = "v0.5/Passive", DefaultName = "对指定卡造成伤害")]
        public class ModelType : BehaviorModel
        {
            public int DamageToDeal
            {
                get; set;
            }
        }
    }
}
