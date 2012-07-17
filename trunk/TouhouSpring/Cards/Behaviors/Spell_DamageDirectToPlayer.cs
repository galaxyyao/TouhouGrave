using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public class Spell_DamageDirectToPlayer:
        BaseBehavior<Spell_DamageDirectToPlayer.ModelType>,
        ICastableSpell
    {
        public bool Cast(Game game, out string reason)
        {
            if (!game.PlayerPlayer.IsSkillCharged)
            {
                reason = "主角技能还没有被充能！";
                return false;
            }

            using (new IntegerEx.LockValues())
            {
                game.UpdateHealth(game.OpponentPlayer, -Model.Damage, this);
                reason = String.Empty;
                return true;
            }
        }

        [BehaviorModel(typeof(Spell_DamageDirectToPlayer), DefaultName = "五道难题")]
        public class ModelType : BehaviorModel
        {
            public int Damage { get; set; }
        }
    }
}
