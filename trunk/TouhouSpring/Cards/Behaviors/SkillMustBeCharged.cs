using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public class SkillMustBeCharged : BaseBehavior<SkillMustBeCharged.ModelType>,
        IPrerequisiteTrigger<Commands.CastSpell>,
        IEpilogTrigger<Commands.CastSpell>
    {
        CommandResult IPrerequisiteTrigger<Commands.CastSpell>.Run(CommandContext<Commands.CastSpell> context)
        {
            if (context.Command.Spell.Host == Host && !Host.Owner.IsSkillCharged)
            {
                return CommandResult.Cancel("主角技能还没有被充能！");
            }

            return CommandResult.Pass;
        }

        void IEpilogTrigger<Commands.CastSpell>.Run(CommandContext<Commands.CastSpell> context)
        {
            if (context.Command.Spell.Host == Host && Model.Discharge)
            {
                context.Game.IssueCommands(new Commands.Discharge
                {
                    Player = Host.Owner
                });
            }
        }

        [BehaviorModel(typeof(SkillMustBeCharged))]
        public class ModelType : BehaviorModel
        {
            public bool Discharge { get; set; }
        }
    }
}
