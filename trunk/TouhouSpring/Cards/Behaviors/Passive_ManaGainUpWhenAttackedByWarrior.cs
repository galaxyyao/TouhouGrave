using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_ManaGainUpWhenAttackedByWarrior
        : BaseBehavior<Passive_ManaGainUpWhenAttackedByWarrior.ModelType>,
        IEpilogTrigger<Commands.DealDamageToPlayer>,
        IPrologTrigger<Commands.UpdateMana>
    {
        private bool isAttackedByWarriorLastRound = false;

        void IEpilogTrigger<Commands.DealDamageToPlayer>.Run(Commands.DealDamageToPlayer command)
        {
            if (command.Player == Host.Owner
                && command.Cause is Warrior
                && !(command.Cause as Warrior).Host.Behaviors.Has<Hero>())
            {
                isAttackedByWarriorLastRound = true;
            }
        }

        void IPrologTrigger<Commands.UpdateMana>.Run(Commands.UpdateMana command)
        {
            if (command.Cause is Game && Game.CurrentPhase == "PhaseB"
                && command.Player == Host.Owner
                && isAttackedByWarriorLastRound)
            {
                command.PatchDeltaAmount(command.DeltaAmount + 1);
                isAttackedByWarriorLastRound = false;
            }
        }

        [BehaviorModel(typeof(Passive_ManaGainUpWhenAttackedByWarrior), DefaultName = "M子")]
        public class ModelType : BehaviorModel
        { }
    }
}
