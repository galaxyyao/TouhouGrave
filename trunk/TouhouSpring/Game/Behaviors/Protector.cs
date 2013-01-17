using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Protector : BaseBehavior<Protector.ModelType>,
        IPrerequisiteTrigger<Commands.PlayCard>
    {
        CommandResult IPrerequisiteTrigger<Commands.PlayCard>.Run(Commands.PlayCard command)
        {
            if (command.CardToPlay == Host
                && Model.Unique
                && Host.Owner.CardsOnBattlefield.Any(card => card.Model == Host.Model))
            {
                return CommandResult.Cancel("Only one protector is allowed on battlefield.");
            }

            return CommandResult.Pass;
        }

        [BehaviorModel(typeof(Protector), Category = "Core")]
        public class ModelType : BehaviorModel
        {
            public bool Unique { get; set; }
        }
    }
}
