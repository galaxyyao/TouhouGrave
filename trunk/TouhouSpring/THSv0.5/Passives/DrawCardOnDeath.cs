using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TouhouSpring.Behaviors;

namespace TouhouSpring.THSv0_5.Passives
{
    public sealed class DrawCardOnDeath : BaseBehavior<DrawCardOnDeath.ModelType>,
        ILocalEpilogTrigger<Commands.IMoveCard>
    {
        void ILocalEpilogTrigger<Commands.IMoveCard>.RunLocalEpilog(Commands.IMoveCard command)
        {
            if (command.ToZone == SystemZone.Graveyard)
            {
                Game.QueueCommands(new Commands.DrawMove(Host.Owner, SystemZone.Hand));
            }
        }

        [BehaviorModel(typeof(DrawCardOnDeath), Category = "v0.5/Passive", DefaultName = "轮回")]
        public class ModelType : BehaviorModel
        { }
    }
}
