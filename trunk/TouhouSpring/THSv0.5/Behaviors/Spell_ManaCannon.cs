using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Spell_ManaCannon:
        BaseBehavior<Spell_ManaCannon.ModelType>,
        Commands.ICause,
        ICastableSpell
    {
        void ICastableSpell.Run(Commands.CastSpell command)
        {
            //TODO: Future change for 3 or more players
            int freeMana=Game.ActingPlayer.FreeMana;
            if (freeMana == 0)
            {
                //TODO: Add failed condition check handling
                new Interactions.MessageBox(Game.ActingPlayer, "没有足够的灵力来施放", Interactions.MessageBoxButtons.OK);
            }
            if (Game.ActingPlayerEnemies.First().CardsOnBattlefield.Count == 0)
            {
                new Interactions.MessageBox(Game.ActingPlayer, "没有可以施放的对象", Interactions.MessageBoxButtons.OK);
            }
            
            var selectedCard = new Interactions.SelectCards(
                Game.ActingPlayerEnemies.First()
                , Game.ActingPlayerEnemies.First().CardsOnBattlefield.Where(card => card.Behaviors.Has<Warrior>()).ToArray().ToIndexable()
                , Interactions.SelectCards.SelectMode.Single
                , "指定1张对手的卡，造成伤害"
                ).Run();
            BaseCard castTarget = selectedCard[0];
            
            Game.IssueCommands(new Commands.UpdateMana(Host.Owner, -freeMana, this));
            Game.IssueCommands(new Commands.DealDamageToCard(castTarget, freeMana, this));
        }

        [BehaviorModel(typeof(Spell_ManaCannon), DefaultName = "灵力炮")]
        public class ModelType : BehaviorModel
        {
        }
    }
}
