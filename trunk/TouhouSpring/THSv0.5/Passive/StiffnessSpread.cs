using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors.Passive
{
    public sealed class StiffnessSpread : BaseBehavior<StiffnessSpread.ModelType>, IStatusEffect,
        IGlobalEpilogTrigger<Commands.DealDamageToCard>
    {
        public string IconUri { get { return "atlas:Textures/Icons/Icons0$BTNAnimateDead"; } }
        public string Text { get { return "传染\n受到此卡攻击时获得僵硬效果。"; } }

        void IGlobalEpilogTrigger<Commands.DealDamageToCard>.RunGlobalEpilog(Commands.DealDamageToCard command)
        {
            if (command.Cause is Warrior
                && command.Cause == Host.Warrior)
            {
                Game.QueueCommands(
                    new Commands.AddBehavior(command.Target, new Stiffness.ModelType().CreateInitialized()),
                    new Commands.AddBehavior(command.Target, Model.CreateInitialized()));
            }
        }

        [BehaviorModel(typeof(StiffnessSpread), Category = "v0.5/Passive", DefaultName = "传染僵硬")]
        public class ModelType : BehaviorModel
        {
            public Stiffness.ModelType StiffnessModel { get; private set; }

            public ModelType()
            {
                StiffnessModel = new Stiffness.ModelType();
            }
        }
    }
}
