﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_DrawCardOnPlay:
        BaseBehavior<Passive_DrawCardOnPlay.ModelType>,
        Commands.ICause,
        IEpilogTrigger<Commands.PlayCard>
    {
        public void RunEpilog(Commands.PlayCard command)
        {
            if (command.CardToPlay == Host)
                Game.QueueCommands(new Commands.DrawCard(Host.Owner));
        }

        [BehaviorModel(typeof(Passive_DrawCardOnPlay), Category = "v0.5/Passive", DefaultName = "上场抽卡")]
        public class ModelType : BehaviorModel
        {
        }
    }
}
