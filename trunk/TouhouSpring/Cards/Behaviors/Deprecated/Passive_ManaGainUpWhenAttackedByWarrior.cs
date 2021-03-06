﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_ManaGainUpWhenAttackedByWarrior
        : BaseBehavior<Passive_ManaGainUpWhenAttackedByWarrior.ModelType>,
        Commands.ICause,
        IGlobalEpilogTrigger<Commands.SubtractPlayerLife>,
        IGlobalEpilogTrigger<Commands.EndPhase>
    {
        private bool isAttackedByWarriorLastRound = false;

        public void RunGlobalEpilog(Commands.SubtractPlayerLife command)
        {
            if (command.Player == Host.Owner
                && command.Cause is Warrior
                && !(command.Cause as Warrior).Host.Behaviors.Has<Hero>())
            {
                isAttackedByWarriorLastRound = true;
            }
        }

        public void RunGlobalEpilog(Commands.EndPhase command)
        {
            if (command.PreviousPhase == "Upkeep"
                && Game.ActingPlayer == Host.Owner
                && isAttackedByWarriorLastRound)
            {
                Game.QueueCommands(new Commands.AddPlayerMana(Host.Owner, 1, this));
                isAttackedByWarriorLastRound = false;
            }
        }

        protected override void OnTransferFrom(IBehavior original)
        {
            isAttackedByWarriorLastRound = (original as Passive_ManaGainUpWhenAttackedByWarrior).isAttackedByWarriorLastRound;
        }

        [BehaviorModel(typeof(Passive_ManaGainUpWhenAttackedByWarrior), DefaultName = "M子")]
        public class ModelType : BehaviorModel
        { }
    }
}
