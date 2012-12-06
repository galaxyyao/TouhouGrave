using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public class MasterSpark : BaseBehavior<MasterSpark.ModelType>, ICastableSpell
    {
        public bool Cast(Game game, out string reason)
        {
            if (!game.PlayerPlayer.IsSkillCharged)
            {
                reason = "Master Spark has not been charged!";
                return false;
            }

            using (new IntegerEx.LockValues())
            {
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
        }

        [BehaviorModel(typeof(MasterSpark), Category = "Deprecated", DefaultName = "Master Spark")]
        public class ModelType : BehaviorModel
        {
            public int Damage { get; set; }
        }
    }

}
