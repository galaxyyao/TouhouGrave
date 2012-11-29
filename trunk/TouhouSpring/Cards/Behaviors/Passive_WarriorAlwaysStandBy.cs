using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public class Passive_WarriorAlwaysStandBy:
        BaseBehavior<Passive_WarriorAlwaysStandBy.ModelType>,
        Commands.IEpilogTrigger<Commands.EndTurn>
    {
        void Commands.IEpilogTrigger<Commands.EndTurn>.Run(Commands.CommandContext context)
        {
            if (IsOnBattlefield)
            {
                throw new NotImplementedException();
                // TODO: issue command for the following:
                //context.Game.SetWarriorState(Host, WarriorState.StandingBy);
            }
        }

        [BehaviorModel(typeof(Passive_WarriorAlwaysStandBy))]
        public class ModelType : BehaviorModel
        { }
    }
}
