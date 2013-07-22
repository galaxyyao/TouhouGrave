using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TouhouSpring.Behaviors;

namespace TouhouSpring.THSv0_5.Passives
{
    public sealed class EffectOnAttack : BaseBehavior<EffectOnAttack.ModelType>,
        IGlobalEpilogTrigger<Commands.DealDamageToCard>
    {
        void IGlobalEpilogTrigger<Commands.DealDamageToCard>.RunGlobalEpilog(Commands.DealDamageToCard command)
        {
            if (command.Cause == Host.Warrior
                && Host.Warrior != null
                && (!Model.Unique || !command.Target.Behaviors.Any(bhv => bhv.Model == Model.Effect.Value)))
            {
                Game.QueueCommands(
                    new Commands.AddBehavior(command.Target, Model.Effect.Value.CreateInitialized()));
            }
        }

        [BehaviorModel(typeof(EffectOnAttack), Category = "v0.5/Passive", DefaultName = "攻击时给予效果")]
        public class ModelType : BehaviorModel
        {
            public BehaviorModelReference Effect { get; set; }
            public bool Unique { get; set; }
        }
    }
}
