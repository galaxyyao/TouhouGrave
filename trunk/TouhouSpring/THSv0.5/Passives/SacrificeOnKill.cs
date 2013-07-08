using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TouhouSpring.Behaviors;

namespace TouhouSpring.THSv0_5.Passives
{
    public sealed class SacrificeOnKill : BaseBehavior<SacrificeOnKill.ModelType>,
        ILocalPrologTrigger<Commands.IMoveCard>
    {
        void ILocalPrologTrigger<Commands.IMoveCard>.RunLocalProlog(Commands.IMoveCard command)
        {
            if (command.ToZone == SystemZone.Graveyard)
            {
                // TODO: reset the card
                command.PatchZoneMoveTo(SystemZone.Sacrifice);
            }
        }

        [BehaviorModel(typeof(SacrificeOnKill), Category = "v0.5/Passive", DefaultName = "尸解")]
        public class ModelType : BehaviorModel
        {
        }
    }
}
