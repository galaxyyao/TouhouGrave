using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_OnUpkeep_HealSelectedCardFull:
        BaseBehavior<Passive_OnUpkeep_HealSelectedCardFull.ModelType>,
        Commands.ICause,
        IGlobalEpilogTrigger<Commands.EndPhase>
    {
        public void RunGlobalEpilog(Commands.EndPhase command)
        {
            if (command.PreviousPhase == "Upkeep"
                && Game.ActingPlayer == Host.Owner)
            {
                if(Game.ActingPlayer.Mana<=0)
                    return;
                var selectedCard = new Interactions.SelectCards(
                    Game.ActingPlayer,
                    Game.ActingPlayer.CardsOnBattlefield,
                    Interactions.SelectCards.SelectMode.Single
                    ).Run();
                if (selectedCard.Count > 0)
                {
                    int lifeToHeal = selectedCard[0].Behaviors.Get<Warrior>().MaxLife - selectedCard[0].Behaviors.Get<Warrior>().Life;
                    Game.QueueCommands(new Commands.HealCard(selectedCard[0]
                        , lifeToHeal
                        , this));
                    if (lifeToHeal >= 3)
                    {
                        Game.QueueCommands(new Commands.SubtractPlayerMana(Game.ActingPlayer
                            , 1
                            , this));
                    }
                }
            }
        }

        [BehaviorModel(typeof(Passive_OnUpkeep_HealSelectedCardFull), Category = "v0.5/Assist", DefaultName = "单卡完全治愈")]
        public class ModelType : BehaviorModel
        {
        }
    }
}
