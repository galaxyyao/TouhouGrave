using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class ManaCost : BaseBehavior<ManaCost.ModelType>,
        Commands.ICause,
        IPrerequisiteTrigger<Commands.MoveCard<Commands.Hand, Commands.Battlefield>>,
        IPrerequisiteTrigger<Commands.ActivateAssist>
    {
        public int Cost
        {
            get { return Model.Cost; }
        }

        public CommandResult RunPrerequisite(Commands.MoveCard<Commands.Hand, Commands.Battlefield> command)
        {
            if (command.Subject == Host)
            {
                Game.NeedMana(Model.Cost);
            }

            return CommandResult.Pass;
        }

        public CommandResult RunPrerequisite(Commands.ActivateAssist command)
        {
            if (command.CardToActivate == Host)
            {
                Game.NeedMana(Model.Cost);
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
