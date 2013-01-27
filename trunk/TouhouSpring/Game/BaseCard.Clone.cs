using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
    public partial class BaseCard
    {
        internal BaseCard CloneForSimulation(Player owner)
        {
            var clonedCard = new BaseCard
            {
                Model = Model,
                Owner = owner
            };
            clonedCard.Behaviors = new Behaviors.BehaviorList(clonedCard);

            for (int i = 0; i < Behaviors.Count; ++i)
            {
                var bhvType = Behaviors[i].GetType();
                var bhv = bhvType.Assembly.CreateInstance(bhvType.FullName) as Behaviors.IBehavior;
                clonedCard.Behaviors.Add(bhv);
            }

            return clonedCard;
        }

        internal void TransferFrom(BaseCard original)
        {
            for (int i = 0; i < Behaviors.Count; ++i)
            {
                (Behaviors[i] as Behaviors.IInternalBehavior).TransferFrom(original.Behaviors[i]);
            }
        }

        private BaseCard() { }
    }
}
