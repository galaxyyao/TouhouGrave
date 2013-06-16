﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class ManaOrLifeCost : BaseBehavior<ManaOrLifeCost.ModelType>,
        Commands.ICause,
        IPrerequisiteTrigger<Commands.MoveCard<Commands.Hand, Commands.Battlefield>>
    {
        public CommandResult RunPrerequisite(Commands.MoveCard<Commands.Hand, Commands.Battlefield> command)
        {
            if (command.Subject == Host)
            {
                Game.NeedManaOrLife(Model.ManaCost, Model.LifeCost);
            }

            return CommandResult.Pass;
        }

        [BehaviorModel(typeof(ManaOrLifeCost), DefaultName="支付灵力或生命", Category="v0.5")]
        public class ModelType : BehaviorModel
        {
            public int ManaCost { get; set; }
            public int LifeCost { get; set; }
        }
    }
}
