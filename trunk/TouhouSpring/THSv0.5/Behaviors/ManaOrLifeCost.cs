using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class ManaOrLifeCost : BaseBehavior<ManaOrLifeCost.ModelType>,
        Commands.ICause,
        IPrerequisiteTrigger<Commands.PlayCard>
    {
        public CommandResult RunPrerequisite(Commands.PlayCard command)
        {
            if (command.CardToPlay == Host)
            {
                Game.NeedManaOrLife(Model.ManaCost, Model.LifeCost);
            }

            return CommandResult.Pass;
        }

        [BehaviorModel(DefaultName="支付灵力或生命", Category="v0.5")]
        public class ModelType : BehaviorModel<ManaOrLifeCost>
        {
            public int ManaCost { get; set; }
            public int LifeCost { get; set; }
        }
    }
}
