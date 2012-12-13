using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_ManaGainUpWhenAttackedByWarrior
        : BaseBehavior<Passive_ManaGainUpWhenAttackedByWarrior.ModelType>,
        IEpilogTrigger<Commands.DealDamageToPlayer>,
        IEpilogTrigger<Commands.StartTurn>
    {
        private bool isAttackedByWarriorLastRound = false;

        void IEpilogTrigger<Commands.DealDamageToPlayer>.Run(Commands.DealDamageToPlayer command)
        {
            if (command.Player == Host.Owner
                && command.Cause is Warrior
                && !(command.Cause as Warrior).Host.Behaviors.Has<Hero>())
            {
                isAttackedByWarriorLastRound = true;

                throw new NotImplementedException();
                // TODO: issue command for the following:
                //Host.Owner.ManaDelta += 1;
            }
        }

        void IEpilogTrigger<Commands.StartTurn>.Run(Commands.StartTurn command)
        {
            if (Game.ActingPlayer != Host.Owner
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
