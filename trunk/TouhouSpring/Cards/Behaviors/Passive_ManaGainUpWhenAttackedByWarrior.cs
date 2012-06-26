using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public class Passive_ManaGainUpWhenAttackedByWarrior
        : BaseBehavior<Passive_ManaGainUpWhenAttackedByWarrior.ModelType>,
        ITrigger<Triggers.PostPlayerDamagedContext>,
        ITrigger<Triggers.PlayerTurnStartedContext>
    {
        private bool isAttackedByWarriorLastRound = false;

        public void Trigger(Triggers.PostPlayerDamagedContext context)
        {
            if (context.PlayerDamaged == Host.Owner && context.Cause.Host.Behaviors.Get<Hero>() == null)
            {
                isAttackedByWarriorLastRound = true;
                Host.Owner.ManaDelta += 1;
            }
        }

        public void Trigger(Triggers.PlayerTurnStartedContext context)
        {
            if (context.Game.PlayerPlayer != Host.Owner
                && isAttackedByWarriorLastRound)
            {
                isAttackedByWarriorLastRound = false;
                Host.Owner.ManaDelta -= 1;
            }
        }

        [BehaviorModel("M子", typeof(Passive_ManaGainUpWhenAttackedByWarrior))]
        public class ModelType : BehaviorModel
        { }
    }
}
