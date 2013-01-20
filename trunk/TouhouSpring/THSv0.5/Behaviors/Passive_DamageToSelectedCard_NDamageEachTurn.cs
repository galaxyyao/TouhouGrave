using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_DamageToSelectedCard_NDamageEachTurn :
        BaseBehavior<Passive_DamageToSelectedCard_NDamageEachTurn.ModelType>,
        Commands.ICause,
        IEpilogTrigger<Commands.DrawCard>
    {
        public void RunEpilog(Commands.DrawCard command)
        {
            //TODO: Need to be modified for 3 or more players' game
            if (Game.ActingPlayer == Host.Owner
                && Game.ActingPlayerEnemies.First().CardsOnBattlefield.Count>0)
            {
                var selectedCard = new Interactions.SelectCards(
                    Host.Owner,
                    Game.ActingPlayerEnemies.First().CardsOnBattlefield.Where(c => c.Behaviors.Has<Warrior>()).ToArray().ToIndexable(),
                    Interactions.SelectCards.SelectMode.Single,
                    "指定1张对手的卡，造成伤害").Run();

                BaseCard castTarget = selectedCard[0];
                Game.IssueCommands(new Commands.DealDamageToCard(castTarget, Model.DamageToDeal, this));
            }
        }

        [BehaviorModel(typeof(Passive_DamageToSelectedCard_NDamageEachTurn), Category = "v0.5/Passive", DefaultName = "对指定卡造成伤害")]
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
