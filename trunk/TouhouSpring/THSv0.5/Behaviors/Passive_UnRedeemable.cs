using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_UnRedeemable:
        BaseBehavior<Passive_UnRedeemable.ModelType>,
        Commands.ICause,
        IPrerequisiteTrigger<Commands.MoveCard<Commands.Sacrifice, Commands.Hand>>
    {
        public CommandResult RunPrerequisite(Commands.MoveCard<Commands.Sacrifice, Commands.Hand> command)
        {
            return command.Subject != Host ? CommandResult.Pass : CommandResult.Cancel();
        }

        [BehaviorModel(typeof(Passive_UnRedeemable), Category = "v0.5/Passive", DefaultName = "不可赎回")]
        public class ModelType : BehaviorModel
        {
        }
    }
}
