using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_UnRedeemable:
        BaseBehavior<Passive_UnRedeemable.ModelType>,
        Commands.ICause,
        ILocalPrerequisiteTrigger<Commands.IInitiativeMoveCard>
    {
        public CommandResult RunLocalPrerequisite(Commands.IInitiativeMoveCard command)
        {
            if (command.FromZone == SystemZone.Sacrifice
                && command.ToZone == SystemZone.Hand
                && command.Cause is Game)
            {
                return CommandResult.Cancel();
            }

            return CommandResult.Pass;
        }

        [BehaviorModel(typeof(Passive_UnRedeemable), Category = "v0.5/Passive", DefaultName = "不可赎回")]
        public class ModelType : BehaviorModel
        {
        }
    }
}
