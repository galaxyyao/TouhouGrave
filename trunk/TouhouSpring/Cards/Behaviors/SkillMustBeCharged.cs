using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class SkillMustBeCharged : BaseBehavior<SkillMustBeCharged.ModelType>,
        IPrerequisiteTrigger<Commands.CastSpell>,
        IEpilogTrigger<Commands.CastSpell>
    {
        CommandResult IPrerequisiteTrigger<Commands.CastSpell>.Run(Commands.CastSpell command)
        {
            if (command.Spell.Host == Host && !Host.Owner.IsSkillCharged)
            {
                return CommandResult.Cancel("主角技能还没有被充能！");
            }

            return CommandResult.Pass;
        }

        void IEpilogTrigger<Commands.CastSpell>.Run(Commands.CastSpell command)
        {
            if (command.Spell.Host == Host && Model.Discharge)
            {
                command.Game.IssueCommands(new Commands.Discharge(Host.Owner));
            }
        }

        [BehaviorModel(typeof(SkillMustBeCharged))]
        public class ModelType : BehaviorModel
        {
            public bool Discharge { get; set; }
        }
    }
}
