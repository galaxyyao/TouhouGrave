﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_RegenerateFull :
        BaseBehavior<Passive_RegenerateFull.ModelType>,
        Commands.ICause,
        IGlobalEpilogTrigger<Commands.StartTurn>
    {
        public void RunGlobalEpilog(Commands.StartTurn command)
        {
            if (Host.Owner == Game.ActingPlayer
                && Host.Warrior != null
                && Host.IsOnBattlefield)
                Game.QueueCommands(
                    new Commands.HealCard(Host, Host.Warrior.MaxLife - Host.Warrior.Life, this));
        }

        [BehaviorModel(typeof(Passive_RegenerateFull), Category = "v0.5/Passive", DefaultName = "单卡完全恢复")]
        public class ModelType : BehaviorModel
        {
        }
    }
}
