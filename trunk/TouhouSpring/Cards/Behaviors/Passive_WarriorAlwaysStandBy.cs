using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_WarriorAlwaysStandBy :
        BaseBehavior<Passive_WarriorAlwaysStandBy.ModelType>,
        IEpilogTrigger<Commands.EndTurn>
    {
        void IEpilogTrigger<Commands.EndTurn>.Run(Commands.EndTurn command)
        {
            if (IsOnBattlefield && Host.Behaviors.Has<Warrior>())
            {
                Game.IssueCommands(new Commands.SendBehaviorMessage(Host.Behaviors.Get<Warrior>(), "GoStandingBy", null));
            }
        }

        [BehaviorModel(typeof(Passive_WarriorAlwaysStandBy))]
        public class ModelType : BehaviorModel
        { }
    }
}
