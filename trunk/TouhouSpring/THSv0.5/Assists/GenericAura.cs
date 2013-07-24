using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TouhouSpring.Behaviors;

namespace TouhouSpring.THSv0_5.Assists
{
    public sealed class GenericAura : Utilities.GenericAura.Behavior<GenericAura.ModelType>,
        ILocalEpilogTrigger<Commands.ActivateAssist>,
        ILocalEpilogTrigger<Commands.DeactivateAssist>
    {
        void ILocalEpilogTrigger<Commands.ActivateAssist>.RunLocalEpilog(Commands.ActivateAssist command)
        {
            OnBeginEffect();
        }

        void ILocalEpilogTrigger<Commands.DeactivateAssist>.RunLocalEpilog(Commands.DeactivateAssist command)
        {
            OnEndEffect();
        }

        [BehaviorModel(typeof(GenericAura), Category = "v0.5/Assist", DefaultName = "进攻光环（支援）")]
        public class ModelType : Utilities.GenericAura.ModelType
        { }
    }
}
