using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_Rebirth : BaseBehavior<Passive_Rebirth.ModelType>,
        ILocalPrologTrigger<Commands.MoveCard>
    {
        void ILocalPrologTrigger<Commands.MoveCard>.RunLocalProlog(Commands.MoveCard command)
        {
            if (command.ToZone == SystemZone.Graveyard)
            {
                command.PatchZoneMoveTo(SystemZone.Hand);
            }
        }

        [BehaviorModel(typeof(Passive_Rebirth), Category = "v0.5/Passive", DefaultName = "磐涅")]
        public class ModelType : BehaviorModel
        {
        }
    }
}
