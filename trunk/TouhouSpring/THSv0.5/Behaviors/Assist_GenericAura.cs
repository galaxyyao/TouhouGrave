using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Assist_GenericAura : GenericAura<Assist_GenericAura.ModelType>,
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

        [BehaviorModel(typeof(Assist_GenericAura), Category = "v0.5/Assist", DefaultName = "进攻光环（支援）")]
        public class ModelType : GenericAuraModelType
        { }
    }
}
