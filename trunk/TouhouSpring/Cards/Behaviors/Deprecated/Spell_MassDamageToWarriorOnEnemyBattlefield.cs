﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Spell_MassDamageToWarriorOnEnemyBattlefield :
        BaseBehavior<Spell_MassDamageToWarriorOnEnemyBattlefield.ModelType>,
        Commands.ICause,
        ICastableSpell
    {
        public void RunSpell(Commands.CastSpell command)
        {
            var warriors = Game.Players.Where(player => player != Host.Owner)
                            .SelectMany(player => player.CardsOnBattlefield)
                            .Where(card => card.Warrior != null);

            foreach (var warrior in warriors)
            {
                Game.QueueCommands(new Commands.DealDamageToCard(warrior, Model.Damage, this));
            }
        }

        [BehaviorModel(typeof(Spell_MassDamageToWarriorOnEnemyBattlefield), DefaultName = "Master Spark")]
        public class ModelType : BehaviorModel
        {
            public int Damage { get; set; }
        }
    }
}
