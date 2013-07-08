using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_OnUpkeep_HealSelectedCardFull :
        BaseBehavior<Passive_OnUpkeep_HealSelectedCardFull.ModelType>,
        Commands.ICause,
        IGlobalEpilogTrigger<Commands.EndPhase>
    {
        public void RunGlobalEpilog(Commands.EndPhase command)
        {
            if (command.PreviousPhase == "Upkeep"
                && Game.ActingPlayer == Host.Owner)
            {
                if (Game.ActingPlayer.Mana <= 0)
                    return;
                var selectedCard = new Interactions.SelectCards(
                    Game.ActingPlayer,
                    Game.ActingPlayer.CardsOnBattlefield.Where(card => card.Warrior != null),
                    Interactions.SelectCards.SelectMode.Single
                    , "指定场上1张角色卡，将其体力回复至上限"
                    ).Run();
                if (selectedCard.Count > 0)
                {
                    int lifeToHeal = selectedCard[0].Warrior.MaxLife - selectedCard[0].Behaviors.Get<Warrior>().Life;
                    Game.QueueCommands(new Commands.HealCard(selectedCard[0]
                        , lifeToHeal
                        , this));
                    if (lifeToHeal >= Model.LifeToHeal)
                    {
                        Game.QueueCommands(new Commands.SubtractPlayerMana(Game.ActingPlayer
                            , Model.ManaToSubstract
                            , this));
                    }
                }
            }
        }

        [BehaviorModel(typeof(Passive_OnUpkeep_HealSelectedCardFull), Category = "v0.5/Assist", DefaultName = "单卡完全治愈")]
        public class ModelType : BehaviorModel
        {
            public int LifeToHeal
            {
                get;
                set;
            }

            public int ManaToSubstract
            {
                get;
                set;
            }
        }
    }
}
