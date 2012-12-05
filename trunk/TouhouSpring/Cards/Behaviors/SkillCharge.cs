using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public class SkillCharge : BaseBehavior<SkillCharge.ModelType>, ICastableSpell
    {
        public bool Cast(Game game, out string reason)
        {
            throw new NotImplementedException();
            // TODO: issue commands for the following:
            //game.PlayerPlayer.IsSkillCharged = true;
            game.IssueCommands(new Commands.Kill
            {
                Target = Host,
                Cause = this
            });

            reason = String.Empty;
            return true;
        }

        [BehaviorModel(typeof(SkillCharge), DefaultName = "补魔")]
        public class ModelType : BehaviorModel
        { }
    }
}
