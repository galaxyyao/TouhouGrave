using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TouhouSpring.Behaviors;

namespace TouhouSpring.THSv0_5.Passives
{
    public sealed class SpellHarmony : BaseBehavior<SpellHarmony.ModelType>,
        IGlobalPrerequisiteTrigger<Commands.PlayCard>
    {
        CommandResult IGlobalPrerequisiteTrigger<Commands.PlayCard>.RunGlobalPrerequisite(Commands.PlayCard command)
        {
            if (command.Subject.Behaviors.Has<Instant>())
            {
                Game.NeedMana(command.Subject.Owner == Host.Owner ? Model.AllySpellCostDelta : Model.EnemySpellCostDelta);
            }

            return CommandResult.Pass;
        }

        [BehaviorModel(typeof(SpellHarmony), Category = "v0.5/Passive", DefaultName = "风水")]
        public class ModelType : BehaviorModel
        {
            public int AllySpellCostDelta { get; set; }
            public int EnemySpellCostDelta { get; set; }

            public ModelType()
            {
                AllySpellCostDelta = -1;
                EnemySpellCostDelta = 1;
            }
        }
    }
}
