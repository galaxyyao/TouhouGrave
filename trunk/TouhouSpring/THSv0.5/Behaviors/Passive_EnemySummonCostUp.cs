﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_EnemySummonCostUp :
        BaseBehavior<Passive_EnemySummonCostUp.ModelType>,
        Commands.ICause,
        IGlobalPrerequisiteTrigger<Commands.PlayCard>
    {
        public CommandResult RunGlobalPrerequisite(Commands.PlayCard command)
        {
            if (command.Subject.Owner != Host.Owner)
            {
                Game.NeedMana(1);
            }
            return CommandResult.Pass;
        }

        [BehaviorModel(typeof(Passive_EnemySummonCostUp), DefaultName = "The World", Category = "v0.5/Passive")]
        public class ModelType : BehaviorModel
        { }
    }
}
