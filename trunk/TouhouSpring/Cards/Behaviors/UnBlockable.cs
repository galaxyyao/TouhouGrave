﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TouhouSpring.Triggers;

namespace TouhouSpring.Behaviors
{
    public class UnBlockable :
        BaseBehavior<UnBlockable.ModelType>,
        ITrigger<BlockPhaseStartedContext>,
        ITrigger<BlockPhaseEndedContext>
    {
        public void Trigger(BlockPhaseStartedContext context)
        {
            if(context.Game.PlayerPlayer==Host.Owner)
                Host.State = CardState.CoolingDown;
        }

        public void Trigger(BlockPhaseEndedContext context)
        {
            if (context.Game.PlayerPlayer == Host.Owner)
                Host.State = CardState.StandingBy;
        }

        [BehaviorModel("不能阻挡", typeof(UnBlockable))]
        public class ModelType : BehaviorModel
        {
        }
    }
}