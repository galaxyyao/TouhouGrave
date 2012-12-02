using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public class Spell_MassDamageToWarriorOnEnemyBattlefield :
        BaseBehavior<Spell_MassDamageToWarriorOnEnemyBattlefield.ModelType>,
        ICastableSpell
    {
        public bool Cast(Game game, out string reason)
        {
            if (!game.PlayerPlayer.IsSkillCharged)
            {
                reason = "主角技能还没有被充能！";
                return false;
            }

            var warriors = (from card in game.OpponentPlayer.CardsOnBattlefield
                            where card.Behaviors.Has<Warrior>()
                            select card.Behaviors.Get<Warrior>()).ToArray();

            foreach (var warrior in warriors)
            {
                warrior.AccumulatedDamage += Model.Damage;
            }

            reason = String.Empty;
            return true;
        }

        [BehaviorModel(typeof(Spell_MassDamageToWarriorOnEnemyBattlefield), DefaultName = "Master Spark")]
        public class ModelType : BehaviorModel
        {
            public int Damage { get; set; }
        }
    }
}
