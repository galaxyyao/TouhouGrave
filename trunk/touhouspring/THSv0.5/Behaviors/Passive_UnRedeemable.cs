using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_UnRedeemable:
        BaseBehavior<Passive_UnRedeemable.ModelType>,
        Commands.ICause,
        IPrerequisiteTrigger<Commands.Redeem>
    {
        public CommandResult RunPrerequisite(Commands.Redeem command)
        {
            if (command.Target == Host)
                return CommandResult.Cancel();
            return CommandResult.Pass;
        }

        [BehaviorModel(typeof(Passive_UnRedeemable), DefaultName = "不可赎回")]
        public class ModelType : BehaviorModel
        {
        }
    }
}
