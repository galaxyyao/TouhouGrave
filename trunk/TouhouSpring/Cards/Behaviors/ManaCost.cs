using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class ManaCost : BaseBehavior<ManaCost.ModelType>,
        Commands.ICause,
        IPrerequisiteTrigger<Commands.PlayCard>,
        IPrerequisiteTrigger<Commands.ActivateAssist>,
        IPrerequisiteTrigger<Commands.Redeem>
    {
        public int Cost
        {
            get { return Model.Cost; }
        }

        public CommandResult RunPrerequisite(Commands.PlayCard command)
        {
            if (command.CardToPlay == Host)
            {
                Game.NeedMana(Host.Owner, Model.Cost);
            }

            return CommandResult.Pass;
        }

        public CommandResult RunPrerequisite(Commands.ActivateAssist command)
        {
            if (command.CardToActivate == Host)
            {
                Game.NeedMana(Host.Owner, Model.Cost);
            }

            return CommandResult.Pass;
        }

        public CommandResult RunPrerequisite(Commands.Redeem command)
        {
            if (command.Target == Host)
            {
                Game.NeedMana(Host.Owner, Model.Cost);
            }

            return CommandResult.Pass;
        }

        [BehaviorModel(typeof(ManaCost))]
        public class ModelType : BehaviorModel
        {
            public int Cost { get; set; }
        }
    }
}
