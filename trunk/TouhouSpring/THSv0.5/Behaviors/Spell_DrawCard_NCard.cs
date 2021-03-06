﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Spell_DrawCard_NCard:
        BaseBehavior<Spell_DrawCard_NCard.ModelType>,
        Commands.ICause,
        ILocalEpilogTrigger<Commands.PlayCard>
    {
        public void RunLocalEpilog(Commands.PlayCard command)
        {
            Model.CardToDraw.Repeat(() => Game.QueueCommands(new Commands.DrawMove(Host.Owner, SystemZone.Hand)));
        }

        [BehaviorModel(typeof(Spell_DrawCard_NCard), Category = "v0.5/Spell")]
        public class ModelType : BehaviorModel
        {
            public int CardToDraw
            {
                get; set;
            }
        }
    }
}
