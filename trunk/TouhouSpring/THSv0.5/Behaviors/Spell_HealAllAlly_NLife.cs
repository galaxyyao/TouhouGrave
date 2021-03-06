﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Spell_HealAllAlly_NLife:
        BaseBehavior<Spell_HealAllAlly_NLife.ModelType>,
        Commands.ICause,
        ILocalEpilogTrigger<Commands.PlayCard>
    {
        public void RunLocalEpilog(Commands.PlayCard command)
        {
            foreach (var card in Game.ActingPlayer.CardsOnBattlefield)
            {
                if (card.Warrior != null)
                {
                    Game.QueueCommands(new Commands.HealCard(card, Model.LifeToHeal, this));
                }
            }
        }

        [BehaviorModel(typeof(Spell_HealAllAlly_NLife), Category = "v0.5/Spell", DefaultName = "全体治愈")]
        public class ModelType : BehaviorModel
        {
            public int LifeToHeal
            {
                get; set;
            }
        }
    }
}
