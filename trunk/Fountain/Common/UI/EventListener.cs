using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TouhouSpring.UI
{
	public class EventListener
	{
		private EventDispatcher m_dispatcher;

		public virtual EventDispatcher Dispatcher
		{
			get { return m_dispatcher; }
			set
			{
				if (value != m_dispatcher)
				{
					if (m_dispatcher != null)
					{
						m_dispatcher.Listeners.Remove(this);
						Debug.Assert(m_dispatcher == null);
					}

					if (value != null)
					{
						value.Listeners.Add(this);
						Debug.Assert(m_dispatcher == value);
					}
				}
			}
		}

		public virtual void RaiseEvent<TEventArgs>(TEventArgs e)
			where TEventArgs : EventArgs
		{
			if (this is IEventListener<TEventArgs>)
			{
				(this as IEventListener<TEventArgs>).RaiseEvent(e);
			}
		}

		internal void InternalSetDispatcher(EventDispatcher dispatcher)
		{
			m_dispatcher = dispatcher;
		}
	}
}
