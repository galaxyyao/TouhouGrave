using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    /// <summary>
    /// Collection of behavior objects.
    /// </summary>
    public class BehaviorList : IIndexable<IBehavior>
    {
        private CardInstance m_host;
        private List<IBehavior> m_behaviors = new List<IBehavior>();

        #region IIndexable<T> interface

        public IBehavior this[int index]
        {
            get { return m_behaviors[index]; }
        }

        public int Count
        {
            get { return m_behaviors.Count; }
        }

        #endregion

        #region IEnumerable<T> interface

        public IEnumerator<IBehavior> GetEnumerator()
        {
            return m_behaviors.GetEnumerator();
        }

        #endregion

        #region IEnumerable interface

        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_behaviors.GetEnumerator();
        }

        #endregion

        public T Get<T>() where T : class, IBehavior
        {
            return m_behaviors.FirstOrDefault(bhv => bhv is T) as T;
        }

        public bool Has<T>() where T : class, IBehavior
        {
            return m_behaviors.Any(bhv => bhv is T);
        }

        internal BehaviorList(CardInstance host)
        {
            if (host == null)
            {
                throw new ArgumentNullException("host");
            }
            m_host = host;
        }

        internal void Reserve(int capacity)
        {
            m_behaviors.Capacity = capacity;
        }

        internal void Add(IBehavior item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            else if (item.Host != null)
            {
                throw new ArgumentException("Item has already been bound.");
            }
            else if (m_behaviors.Contains(item))
            {
                throw new ArgumentException("Item is already in the list.");
            }

            m_behaviors.Add(item);
            Debug.Assert(item.Host == null);
            (item as IInternalBehavior).Host = m_host;

            var warrior = item as Warrior;
            if (warrior != null)
            {
                Debug.Assert(m_host.Warrior == null);
                m_host.Warrior = warrior;
            }
        }

        internal bool Remove(IBehavior item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            int index = m_behaviors.IndexOf(item);
            if (index != -1)
            {
                RemoveAt(index);
                return true;
            }

            return false;
        }

        internal void RemoveAt(int index)
        {
            if (index < 0 || index > m_behaviors.Count)
            {
                throw new ArgumentOutOfRangeException("index", "Index is out of range.");
            }

            var bhv = m_behaviors[index];
            Debug.Assert(bhv.Host == m_host);
            (bhv as IInternalBehavior).Host = null;
            m_behaviors.RemoveAt(index);

            if (m_host.Warrior == bhv)
            {
                m_host.Warrior = null;
            }
        }
    }
}
