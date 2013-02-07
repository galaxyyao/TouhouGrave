using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
    public partial class BaseCard
    {
        internal BaseCard Clone(Player owner)
        {
            var clonedCard = new BaseCard
            {
                Model = Model,
                Owner = owner
            };
            clonedCard.Behaviors = new Behaviors.BehaviorList(clonedCard);

            for (int i = 0; i < Behaviors.Count; ++i)
            {
                clonedCard.Behaviors.Add((Behaviors[i].Model as Behaviors.IInternalBehaviorModel).Instantiate());
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
