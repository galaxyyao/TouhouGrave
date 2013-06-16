﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Instant : BaseBehavior<Instant.ModelType>,
        Commands.ICause,
        IEpilogTrigger<Commands.IMoveTo<Commands.Battlefield>>
    {
        public void RunEpilog(Commands.IMoveTo<Commands.Battlefield> command)
        {
            if (command.Subject == Host)
            {
                Game.QueueCommands(new Commands.KillMove<Commands.Battlefield>(Host, this));
            }
        }

        [BehaviorModel(typeof(Instant), Category = "Core")]
        public class ModelType : BehaviorModel
        { }
    }
}
