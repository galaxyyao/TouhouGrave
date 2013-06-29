using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_DamageToSelectedCard_NDamageEachTurn :
        BaseBehavior<Passive_DamageToSelectedCard_NDamageEachTurn.ModelType>,
        Commands.ICause,
        IGlobalEpilogTrigger<Commands.EndPhase>
    {
        public void RunGlobalEpilog(Commands.EndPhase command)
        {
            if (command.PreviousPhase == "Upkeep"
                && Game.ActingPlayer == Host.Owner)
            {
                var selectedCard = new Interactions.SelectCards(
                    Host.Owner,
                    Game.ActingPlayerEnemies.SelectMany(p => p.CardsOnBattlefield).Where(c => c.Behaviors.Has<Warrior>()),
                    Interactions.SelectCards.SelectMode.Single,
                    "指定1张对手的卡，造成伤害").Run();
                if (selectedCard.Count > 0)
                {
                    Game.QueueCommands(new Commands.DealDamageToCard(selectedCard[0], Model.DamageToDeal, this));
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
