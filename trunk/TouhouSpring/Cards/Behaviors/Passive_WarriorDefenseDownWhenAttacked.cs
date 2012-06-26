using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public class Passive_WarriorDefenseDownWhenAttacked
        : BaseBehavior<Passive_WarriorDefenseDownWhenAttacked.ModelType>,
        ITrigger<Triggers.PostCardDamagedContext>,
        ITrigger<Triggers.PlayerTurnEndedContext>
    {
        private bool isBlockedLastRound = false;

        public void Trigger(Triggers.PostCardDamagedContext context)
        {
            if (context.CardDamaged == Host
                && context.Game.PlayerPlayer != Host.Owner)
                isBlockedLastRound = true;
        }

        public void Trigger(Triggers.PlayerTurnEndedContext context)
        {
            if (context.Game.PlayerPlayer != Host.Owner && isBlockedLastRound)
            {
                isBlockedLastRound = false;
                Func<int, int> defenseMod = y => y - 1;
                Host.Behaviors.Get<Warrior>().Defense.AddModifierToTail(defenseMod);
            }
        }

        [BehaviorModel("日光折射", typeof(Passive_WarriorDefenseDownWhenAttacked))]
        public class ModelType : BehaviorModel
        { }
    }
}
