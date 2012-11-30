using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TouhouSpring.Commands;

namespace TouhouSpring.Behaviors
{
    public class Passive_WarriorAlwaysStandBy:
        BaseBehavior<Passive_WarriorAlwaysStandBy.ModelType>,
        IEpilogTrigger<EndTurn>
    {
        void IEpilogTrigger<EndTurn>.Run(CommandContext<EndTurn> context)
        {
            if (IsOnBattlefield)
            {
                if (Host.Behaviors.Has<Warrior>())
                {
                    context.Game.IssueCommands(new SendBehaviorMessage
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
