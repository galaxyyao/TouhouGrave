using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class SkillCharge : BaseBehavior<SkillCharge.ModelType>,
        Commands.ICause,
        IPrerequisiteTrigger<Commands.CastSpell>,
        ICastableSpell
    {
        CommandResult IPrerequisiteTrigger<Commands.CastSpell>.Run(Commands.CastSpell command)
        {
            if (command.Spell == this && Host.Owner.IsSkillCharged)
            {
                return CommandResult.Cancel("Player is already charged.");
            }

            return CommandResult.Pass;
        }

        void ICastableSpell.Run(Commands.CastSpell command)
        {
            Game.IssueCommands(
                new Commands.Charge(Host.Owner),
                new Commands.Kill(Host, this));
        }

        [BehaviorModel(typeof(SkillCharge), DefaultName = "补魔")]
        public class ModelType : BehaviorModel
        { }
    }
}
