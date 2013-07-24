using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TouhouSpring.Behaviors;

namespace TouhouSpring.THSv0_5.Passives
{
    public sealed class GenericAura : Utilities.GenericAura.Behavior<GenericAura.ModelType>,
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

        [BehaviorModel(typeof(GenericAura), Category = "v0.5/Passive", DefaultName = "进攻光环（被动）")]
        public class ModelType : Utilities.GenericAura.ModelType
        { }
    }
}
