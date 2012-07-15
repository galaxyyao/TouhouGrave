using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public class Passive_WarriorAlwaysStandBy:
        BaseBehavior<Passive_WarriorAlwaysStandBy.ModelType>,
        ITrigger<Triggers.PlayerTurnEndedContext>
    {
        public void Trigger(Triggers.PlayerTurnEndedContext context)
        {
            if (IsOnBattlefield)
            {
                context.Game.SetWarriorState(Host, WarriorState.StandingBy);
            }
        }

        [BehaviorModel(typeof(Passive_WarriorAlwaysStandBy))]
        public class ModelType : BehaviorModel
        { }
    }
}
