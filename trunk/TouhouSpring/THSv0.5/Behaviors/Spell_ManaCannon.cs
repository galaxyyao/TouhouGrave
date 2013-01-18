using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Spell_ManaCannon:
        BaseBehavior<Spell_ManaCannon.ModelType>,
        Commands.ICause,
        IPrerequisiteTrigger<Commands.CastSpell>,
        ICastableSpell
    {
        public CommandResult RunPrerequisite(Commands.CastSpell command)
        {
            int freeMana = Game.ActingPlayer.FreeMana;
            if (freeMana == 0)
            {
                return CommandResult.Cancel("没有足够的灵力来施放");
            }
            if (Game.ActingPlayerEnemies.First().CardsOnBattlefield.Count == 0)
            {
                return CommandResult.Cancel("没有可以施放的对象");
            }
            return CommandResult.Pass;
        }

        public void RunSpell(Commands.CastSpell command)
        {
            //TODO: Future change for 3 or more players
            int freeMana = Game.ActingPlayer.FreeMana;
            
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
