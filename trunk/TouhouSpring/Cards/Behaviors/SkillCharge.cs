using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public class SkillCharge : BaseBehavior<SkillCharge.ModelType>,
        IPrerequisiteTrigger<Commands.CastSpell>,
        ICastableSpell
    {
        CommandResult IPrerequisiteTrigger<Commands.CastSpell>.Run(CommandContext<Commands.CastSpell> context)
        {
            if (context.Command.Spell == this && Host.Owner.IsSkillCharged)
            {
                return CommandResult.Cancel("Player is already charged.");
            }

            return CommandResult.Pass;
        }

        void ICastableSpell.Run(CommandContext<Commands.CastSpell> context)
        {
            context.Game.IssueCommands(
                new Commands.Charge
                {
                    Player = Host.Owner
                },
                new Commands.Kill
                {
                    Target = Host,
                    Cause = this
                });
        }

        [BehaviorModel(typeof(SkillCharge), DefaultName = "补魔")]
        public class ModelType : BehaviorModel
        { }
    }
}
