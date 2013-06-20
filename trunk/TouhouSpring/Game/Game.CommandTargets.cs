using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
    public partial class Game
    {
        internal List<Behaviors.BehaviorList> m_globalTargetLists = new List<Behaviors.BehaviorList>();

        internal void SubscribeCardToCommands(CardInstance card)
        {
            m_globalTargetLists.Add(card.Behaviors);
        }

        internal void UnsubscribeCardFromCommands(CardInstance card)
        {
            m_globalTargetLists.Remove(card.Behaviors);
        }
    }
}
