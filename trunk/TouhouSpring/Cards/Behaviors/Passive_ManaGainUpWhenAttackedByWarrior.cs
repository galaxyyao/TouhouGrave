using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_ManaGainUpWhenAttackedByWarrior
        : BaseBehavior<Passive_ManaGainUpWhenAttackedByWarrior.ModelType>,
        Commands.ICause,
        IEpilogTrigger<Commands.SubtractPlayerLife>,
        IEpilogTrigger<Commands.EndPhase>
    {
        private bool isAttackedByWarriorLastRound = false;

        public void RunEpilog(Commands.SubtractPlayerLife command)
        {
            if (command.Player == Host.Owner
                && command.Cause is Warrior
                && !(command.Cause as Warrior).Host.Behaviors.Has<Hero>())
            {
                isAttackedByWarriorLastRound = true;
            }
        }

        public void RunEpilog(Commands.EndPhase command)
        {
            if (command.PreviousPhase == "Upkeep"
                && Game.ActingPlayer == Host.Owner
                && isAttackedByWarriorLastRound)
            {
                Game.IssueCommands(new Commands.AddPlayerMana(Host.Owner, 1, this));
                isAttackedByWarriorLastRound = false;
            }
        }

        protected override void OnTransferFrom(IBehavior original)
        {
            isAttackedByWarriorLastRound = (original as Passive_ManaGainUpWhenAttackedByWarrior).isAttackedByWarriorLastRound;
        }

        [BehaviorModel(DefaultName = "M子")]
        public class ModelType : BehaviorModel<Passive_ManaGainUpWhenAttackedByWarrior>
        { }
    }
}
