﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Spell_DirectDamage_NDamage :
        BaseBehavior<Spell_DirectDamage_NDamage.ModelType>,
        Commands.ICause,
        IPrerequisiteTrigger<Commands.PlayCard>,
        IPrologTrigger<Commands.PlayCard>
    {
        CommandResult IPrerequisiteTrigger<Commands.PlayCard>.Run(Commands.PlayCard command)
        {
            //TODO: Future change for 3 or more players
            if (Game.ActingPlayerEnemies.First().CardsOnBattlefield.Count == 0)
            {
                return CommandResult.Cancel("没有可以被指定为对象的卡");
            }

            return CommandResult.Pass;
        }

        //TODO: Future change for instant handling
        void IPrologTrigger<Commands.PlayCard>.Run(Commands.PlayCard command)
        {
            var selectedCard = new Interactions.SelectCards(
                Game.ActingPlayerEnemies.First(),
                Game.ActingPlayerEnemies.First().CardsOnBattlefield.Where(card => card.Behaviors.Has<Warrior>()).ToArray().ToIndexable(),
                Interactions.SelectCards.SelectMode.Single,
                "指定1张对手的卡，造成伤害"
                ).Run();
            BaseCard castTarget = selectedCard[0];
            Game.IssueCommands(new Commands.DealDamageToCard(castTarget, Model.DamageToDeal, this));
        }

        [BehaviorModel(typeof(Spell_DirectDamage_NDamage), DefaultName = "直接伤害")]
        public class ModelType : BehaviorModel
        {
            public int DamageToDeal
            { get; set; }
        }
    }
}