using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public class Passive_WarriorAlwaysStandBy:
        BaseBehavior<Passive_WarriorAlwaysStandBy.ModelType>,
        IEpilogTrigger<Commands.EndTurn>
    {
        void IEpilogTrigger<Commands.EndTurn>.Run(CommandContext<Commands.EndTurn> context)
        {
            if (IsOnBattlefield)
            {
                if (Host.Behaviors.Has<Warrior>())
                {
                    context.Game.IssueCommands(new Commands.SendBehaviorMessage
                    {
                        Target = Host.Behaviors.Get<Warrior>(),
                        Message = "GoStandingBy"
                    });
                }
            }
        }

        [BehaviorModel(typeof(Passive_WarriorAlwaysStandBy))]
        public class ModelType : BehaviorModel
        { }
    }
}
