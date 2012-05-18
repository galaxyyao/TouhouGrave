using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.UI
{
	public class EventListenerList : IList<EventListener>
	{
		public class EventArgs : System.EventArgs
		{
			public EventListener EventListener
			{
				get; private set;
			}

			public EventArgs(EventListener eventListener)
			{
				EventListener = eventListener;
			}
		}

		private EventDispatcher m_dispatcher;
		private List<EventListener> m_eventListeners = new List<EventListener>();

		#region IList<T> interface

		public EventListener this[int index]
		{
			get { return m_eventListeners[index]; }
			set
			{
				if (index < 0 || index > m_eventListeners.Count)
				{
					throw new ArgumentOutOfRangeException("Index is out of range.");
				}
				else if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				else if (m_eventListeners.Contains(value))
				{
					throw new ArgumentException("Item is already in the list.");
				}
				else if (value.Dispatcher != null)
				{
					throw new InvalidOperationException("Item is parented by another dispatcher.");
				}

				var old = m_eventListeners[index];
				if (old != value)
				{
					m_eventListeners[index] = value;
					old.InternalSetDispatcher(null);
					RaiseEvent(ItemRemoved, old);

					value.InternalSetDispatcher(m_dispatcher);
					RaiseEvent(ItemAdded, value);
				}
			}
		}

		public int IndexOf(EventListener item)
		{
			return m_eventListeners.IndexOf(item);
		}

		public void Insert(int index, EventListener item)
		{
			if (index < 0 || index > m_eventListeners.Count)
			{
				throw new ArgumentOutOfRangeException("Index is out of range.");
			}
			else if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			else if (m_eventListeners.Contains(item))
			{
				throw new ArgumentException("Item is already in the list.");
			}
			else if (item.Dispatcher != null)
			{
				throw new InvalidOperationException("Item is parented by another dispatcher.");
			}

			m_eventListeners.Insert(index, item);
			item.InternalSetDispatcher(m_dispatcher);
			RaiseEvent(ItemAdded, item);
		}

		public void RemoveAt(int index)
		{
			if (index < 0 || index > m_eventListeners.Count)
			{
				throw new ArgumentOutOfRangeException("Index is out of range.");
			}

			var item = m_eventListeners[index];
			m_eventListeners.RemoveAt(index);
			item.InternalSetDispatcher(null);
			RaiseEvent(ItemRemoved, item);
		}

		#endregion

		#region ICollection<T> interface

		public int Count
		{
			get { return m_eventListeners.Count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public void Add(EventListener item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			else if (m_eventListeners.Contains(item))
			{
				throw new ArgumentException("Item is already in the list.");
			}
			else if (item.Dispatcher != null)
			{
				throw new InvalidOperationException("Item is parented by another dispatcher.");
			}

			m_eventListeners.Add(item);
			item.InternalSetDispatcher(m_dispatcher);
			RaiseEvent(ItemAdded, item);
		}

		public void Clear()
		{
			List<EventListener> eventListeners = m_eventListeners;
			m_eventListeners = new List<EventListener>();
			foreach (EventListener eventListener in eventListeners)
			{
				eventListener.InternalSetDispatcher(null);
				RaiseEvent(ItemRemoved, eventListener);
			}
		}

		public bool Contains(EventListener item)
		{
			return m_eventListeners.Contains(item);
		}

		public void CopyTo(EventListener[] array, int arrayIndex)
		{
			m_eventListeners.CopyTo(array, arrayIndex);
		}

		public bool Remove(EventListener item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}

			if (m_eventListeners.Remove(item))
			{
				item.InternalSetDispatcher(null);
				RaiseEvent(ItemRemoved, item);
				return true;
			}

			return false;
		}

		#endregion

		#region IEnumerable<T> interface

		public IEnumerator<EventListener> GetEnumerator()
		{
			return m_eventListeners.GetEnumerator();
		}

		#endregion

		#region IEnumerable interface

		IEnumerator IEnumerable.GetEnumerator()
		{
			return m_eventListeners.GetEnumerator();
		}

		#endregion

		public event EventHandler<EventArgs> ItemAdded;
		public event EventHandler<EventArgs> ItemRemoved;

		internal EventListenerList(EventDispatcher dispatcher)
		{
			if (dispatcher == null)
			{
				throw new ArgumentNullException("dispatcher");
			}

			m_dispatcher = dispatcher;
		}

		private void RaiseEvent(EventHandler<EventArgs> handler, EventListener eventListener)
		{
			if (handler != null)
			{
				handler(this, new EventArgs(eventListener));
			}
		}
	}
}
