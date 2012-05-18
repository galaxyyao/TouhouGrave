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
            // TODO: support game among more than 2 players
            //Player opponentPlayer = game.Players[Host.Owner == game.Players[0] ? 1 : 0];
            if (game.PlayerPlayer.Mana < Model.ManaCost)
            {
                reason = "Insufficient mana";
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

        [BehaviorModel("Master Spark", typeof(MasterSpark))]
        public class ModelType : BehaviorModel
        {
            public int ManaCost { get; set; }
            public int Damage { get; set; }
        }
    }

}
