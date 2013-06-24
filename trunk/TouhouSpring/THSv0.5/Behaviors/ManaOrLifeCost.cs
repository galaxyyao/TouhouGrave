using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class ManaOrLifeCost : BaseBehavior<ManaOrLifeCost.ModelType>,
        Commands.ICause,
        ILocalPrerequisiteTrigger<Commands.PlayCard>
    {
        public CommandResult RunLocalPrerequisite(Commands.PlayCard command)
        {
            Game.NeedManaOrLife(Model.ManaCost, Model.LifeCost);
            return CommandResult.Pass;
        }

        [BehaviorModel(typeof(ManaOrLifeCost), DefaultName="支付灵力或生命", Category="v0.5")]
        public class ModelType : BehaviorModel
        {
            public int ManaCost { get; set; }
            public int LifeCost { get; set; }
        }
    }
}
