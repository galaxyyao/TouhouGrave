﻿using System;
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
                Guid = Guid,
                Zone = Zone
            };

            clonedCard.Behaviors.Capacity = Behaviors.Capacity;
            for (int i = 0; i < Behaviors.Count; ++i)
            {
                var bhv = Behaviors[i];
                clonedCard.Behaviors.Add(bhv.IsStatic ? bhv : (bhv.Model as Behaviors.IInternalBehaviorModel).Instantiate());
            }

            clonedCard.m_counters.Capacity = m_counters.Capacity;
            foreach (var item in m_counters)
            {
                clonedCard.m_counters.Add(item);
            }

            return clonedCard;
        }

        internal void TransferFrom(CardInstance original)
        {
            for (int i = 0; i < Behaviors.Count; ++i)
            {
                var bhv = Behaviors[i];
                if (!bhv.IsStatic)
                {
                    (bhv as Behaviors.IInternalBehavior).TransferFrom(original.Behaviors[i]);
                }
            }
        }
    }
}
