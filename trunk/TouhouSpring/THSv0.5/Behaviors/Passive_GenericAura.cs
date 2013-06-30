using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_GenericAura : GenericAura<Passive_GenericAura.ModelType>,
        ILocalEpilogTrigger<Commands.IMoveCard>
    {
        void ILocalEpilogTrigger<Commands.IMoveCard>.RunLocalEpilog(Commands.IMoveCard command)
        {
            if (command.FromZoneType != ZoneType.OnBattlefield
                && command.ToZoneType == ZoneType.OnBattlefield)
            {
                OnBeginEffect();
            }
            else if (command.FromZoneType == ZoneType.OnBattlefield
                     && command.ToZoneType != ZoneType.OnBattlefield)
            {
                OnEndEffect();
            }
        }

        [BehaviorModel(typeof(Passive_GenericAura), Category = "v0.5/Passive", DefaultName = "进攻光环（被动）")]
        public class ModelType : GenericAuraModelType
        { }
    }
}
