using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public class Passive_WarriorUnblockedable:
        BaseBehavior<Passive_WarriorUnblockedable.ModelType>,
        ITrigger<Triggers.BlockPhaseStartedContext>,
        ITrigger<Triggers.BlockPhaseEndedContext>
    {
        public void Trigger(Triggers.BlockPhaseStartedContext context)
        {
            if (context.Game.PlayerPlayer == Host.Owner)
                Host.State = CardState.CoolingDown;
        }

        public void Trigger(Triggers.BlockPhaseEndedContext context)
        {
            if (context.Game.PlayerPlayer == Host.Owner)
                Host.State = CardState.StandingBy;
        }

        [BehaviorModel("月光", typeof(Passive_WarriorUnblockedable))]
        public class ModelType : BehaviorModel
        { }
    }
}
