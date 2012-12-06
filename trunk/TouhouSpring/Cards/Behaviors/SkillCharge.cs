using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public class SkillCharge : BaseBehavior<SkillCharge.ModelType>, ICastableSpell
    {
        void ICastableSpell.Run(CommandContext<Commands.CastSpell> context)
        {
            throw new NotImplementedException();
            // TODO: issue commands for the following:
            //game.PlayerPlayer.IsSkillCharged = true;
            context.Game.IssueCommands(new Commands.Kill
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
