using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
    public partial class Game
    {
        internal List<Behaviors.IBehavior> m_commonTargets = new List<Behaviors.IBehavior>();

        internal void SubscribeCardToCommands(CardInstance card)
        {
            m_commonTargets.AddRange(card.Behaviors);
        }

        internal void UnsubscribeCardFromCommands(CardInstance card)
        {
            m_commonTargets.RemoveRange(Math.Max(m_commonTargets.IndexOf(card.Behaviors.FirstOrDefault()), 0), card.Behaviors.Count);
        }

        internal void SubscribeBehaviorToCommands(CardInstance card, Behaviors.IBehavior behavior)
        {
            if (card.Owner.ActivatedAssits.Contains(card)
                || card.Owner.m_battlefieldCards.Contains(card))
            {
                if (card.Behaviors.Count > 1)
                {
                    m_commonTargets.Insert(m_commonTargets.IndexOf(card.Behaviors[card.Behaviors.Count - 2]) + 1, behavior);
                }
                else
                {
                    m_commonTargets.Add(behavior);
                }
            }
        }

        internal void UnsubscribeBehaviorFromCommands(CardInstance card, Behaviors.IBehavior behavior)
        {
            if (card.Owner.ActivatedAssits.Contains(card)
                || card.Owner.m_battlefieldCards.Contains(card))
            {
                m_commonTargets.Remove(behavior);
            }
        }
    }
}
