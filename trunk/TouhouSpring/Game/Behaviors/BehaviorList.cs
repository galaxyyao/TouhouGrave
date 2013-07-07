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
        private List<IBehavior> m_behaviors = new List<IBehavior>();

        public CardInstance Host
        {
            get; private set;
        }

        #region IIndexable<T> interface

        public IBehavior this[int index]
        {
            get { return m_behaviors[index]; }
        }

        public int Count
        {
            get { return m_behaviors.Count; }
        }

        public int Capacity
        {
            get { return m_behaviors.Capacity; }
            set { m_behaviors.Capacity = value; }
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
            foreach (var bhv in m_behaviors)
            {
                var t = bhv as T;
                if (t != null)
                {
                    return t;
                }
            }

            return null;
        }

        public bool Has<T>() where T : class, IBehavior
        {
            foreach (var bhv in m_behaviors)
            {
                if (bhv is T)
                {
                    return true;
                }
            }
            return false;
        }

        internal BehaviorList(CardInstance host)
        {
            if (host == null)
            {
                throw new ArgumentNullException("host");
            }
            Host = host;
        }

        internal void Add(IBehavior item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            else if (m_behaviors.Contains(item))
            {
                throw new ArgumentException("Item is already in the list.");
            }

            var bhv = item as IInternalBehavior;
            if (bhv.RealHost != null)
            {
                throw new ArgumentException("Item has already been bound.");
            }

            m_behaviors.Add(bhv);
            if (!bhv.IsStatic)
            {
                bhv.RealHost = Host;
            }

            var warrior = item as Warrior;
            if (warrior != null)
            {
                Debug.Assert(Host.Warrior == null);
                Host.Warrior = warrior;
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

            var bhv = m_behaviors[index] as IInternalBehavior;
            if (bhv.IsStatic)
            {
                Debug.Assert(bhv.RealHost == null);
            }
            else
            {
                Debug.Assert(bhv.RealHost == Host);
                bhv.RealHost = null;
            }
            m_behaviors.RemoveAt(index);

            if (Host.Warrior == bhv)
            {
                Host.Warrior = null;
            }
        }
    }
}
