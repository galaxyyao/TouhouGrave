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
            game.PlayerPlayer.IsSkillCharged = true;

            game.DestroyCard(Host);

            reason = String.Empty;
            return true;
        }

        [BehaviorModel("补魔", typeof(SkillCharge))]
        public class ModelType : BehaviorModel
        { }
    }
}
