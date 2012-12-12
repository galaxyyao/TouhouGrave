using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Spell_MassDamageToWarriorOnEnemyBattlefield :
        BaseBehavior<Spell_MassDamageToWarriorOnEnemyBattlefield.ModelType>,
        ICastableSpell
    {
        void ICastableSpell.Run(Commands.CastSpell command)
        {
            var warriors = Game.Players.Where(player => player != Host.Owner)
                            .SelectMany(player => player.CardsOnBattlefield)
                            .Where(card => card.Behaviors.Has<Warrior>());

            foreach (var warrior in warriors)
            {
                Game.IssueCommands(new Commands.DealDamageToCard(warrior, this, Model.Damage));
            }
        }

        [BehaviorModel(typeof(Spell_MassDamageToWarriorOnEnemyBattlefield), DefaultName = "Master Spark")]
        public class ModelType : BehaviorModel
        {
            public int Damage { get; set; }
        }
    }
}
