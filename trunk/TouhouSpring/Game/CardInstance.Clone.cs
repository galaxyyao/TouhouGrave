using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
    public partial class CardInstance
    {
        internal CardInstance Clone(Player owner)
        {
            var clonedCard = new CardInstance
            {
                Model = Model,
                Owner = owner,
                Guid = Guid
            };

            clonedCard.Behaviors.Reserve(Behaviors.Count);
            for (int i = 0; i < Behaviors.Count; ++i)
            {
                clonedCard.Behaviors.Add((Behaviors[i].Model as Behaviors.IInternalBehaviorModel).Instantiate());
            }

            return clonedCard;
        }

        internal void TransferFrom(CardInstance original)
        {
            for (int i = 0; i < Behaviors.Count; ++i)
            {
                (Behaviors[i] as Behaviors.IInternalBehavior).TransferFrom(original.Behaviors[i]);
            }
        }
    }
}
