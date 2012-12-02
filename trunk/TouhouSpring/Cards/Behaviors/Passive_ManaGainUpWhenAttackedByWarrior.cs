using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TouhouSpring.Commands;

namespace TouhouSpring.Behaviors
{
    public class Passive_ManaGainUpWhenAttackedByWarrior
        : BaseBehavior<Passive_ManaGainUpWhenAttackedByWarrior.ModelType>,
        ITrigger<Triggers.PostPlayerDamagedContext>,
        IEpilogTrigger<StartTurn>
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

        void IEpilogTrigger<StartTurn>.Run(CommandContext<StartTurn> context)
        {
            if (context.Game.PlayerPlayer != Host.Owner
                && isAttackedByWarriorLastRound)
            {
                isAttackedByWarriorLastRound = false;
                throw new NotImplementedException();
                // TODO: issue command for the following:
                //Host.Owner.ManaDelta -= 1;
            }
        }

        [BehaviorModel(typeof(Passive_ManaGainUpWhenAttackedByWarrior), DefaultName = "M子")]
        public class ModelType : BehaviorModel
        { }
    }
}
