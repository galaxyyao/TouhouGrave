using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
    public partial class CardInstance
    {
        private struct CounterItem
        {
            public Behaviors.ICounter Counter;
            public int Count;
        }

        private List<CounterItem> m_counters = new List<CounterItem>();

        /// <summary>
        /// A collection of behaviors bound to this card
        /// </summary>
        public Behaviors.BehaviorList Behaviors
        {
            get;
            private set;
        }

        /// <summary>
        /// Get a collection of behaviors which implements ISpell.
        /// </summary>
        public IEnumerable<Behaviors.ICastableSpell> Spells
        {
            get { return Behaviors.OfType<Behaviors.ICastableSpell>(); }
        }

        public IEnumerable<Behaviors.ICounter> Counters
        {
            get { return m_counters.Select(item => item.Counter); }
        }

        public int GetCounterCount<TCounter>() where TCounter : Behaviors.ICounter
        {
            return GetCounterCount(typeof(TCounter));
        }

        public int GetCounterCount(Type counterType)
        {
            if (counterType == null)
            {
                throw new ArgumentNullException("counterType");
            }

            foreach (var item in m_counters)
            {
                if (item.Counter.GetType() == counterType)
                {
                    return item.Count;
                }
            }
            return 0;
        }

        internal void AddCounter(Behaviors.ICounter counter, int num)
        {
            for (int i = 0; i < m_counters.Count; ++i)
            {
                if (m_counters[i].Counter == counter)
                {
                    m_counters[i] = new CounterItem { Counter = counter, Count = m_counters[i].Count + num };
                    return;
                }
                else if (m_counters.GetType() == counter.GetType())
                {
                    throw new InvalidOperationException(String.Format("Find different counter instances of same type {0}", counter.GetType().FullName));
                }
            }

            m_counters.Add(new CounterItem
            {
                Counter = counter,
                Count = num
            });
        }

        internal void RemoveCounter(Behaviors.ICounter counter, int num)
        {
            for (int i = 0; i < m_counters.Count; ++i)
            {
                if (m_counters[i].Counter == counter)
                {
                    m_counters[i] = new CounterItem { Counter = counter, Count = m_counters[i].Count - num };
                    if (m_counters[i].Count <= 0)
                    {
                        m_counters.RemoveAt(i);
                    }
                    return;
                }
            }

            throw new InvalidOperationException(String.Format("Counter of type {0} can't be found.", counter.GetType().FullName));
        }
    }
}
