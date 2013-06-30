using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class ManaCost : BaseBehavior<ManaCost.ModelType>,
        Commands.ICause,
        ILocalPrerequisiteTrigger<Commands.PlayCard>,
        ILocalPrerequisiteTrigger<Commands.ActivateAssist>
    {
        public int Cost
        {
            get { return Model.Cost; }
        }

        public CommandResult RunLocalPrerequisite(Commands.PlayCard command)
        {
            Game.NeedMana(Model.Cost);
            return CommandResult.Pass;
        }

        public CommandResult RunLocalPrerequisite(Commands.ActivateAssist command)
        {
            Game.NeedMana(Model.Cost);
            return CommandResult.Pass;
        }

        [BehaviorModel(typeof(ManaCost), Category = "Core", Description = "Mana required for summon or cast")]
        public class ModelType : BehaviorModel
        {
            public int Cost { get; set; }
        }
    }
}
