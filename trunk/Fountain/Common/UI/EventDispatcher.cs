using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.UI
{
	public class EventDispatcher : EventListener
	{
		public EventListenerList Listeners
		{
			get; private set;
		}

		public EventDispatcher()
		{
			Listeners = new EventListenerList(this);
		}

		public override void RaiseEvent<TEventArgs>(TEventArgs e)
		{
			if (this is IEventListener<TEventArgs>)
			{
				(this as IEventListener<TEventArgs>).RaiseEvent(e);
			}
			else
			{
				Dispatch(e);
			}
		}

		protected void Dispatch<TEventArgs>(TEventArgs e)
			where TEventArgs : EventArgs
		{
			Action<EventListener> action = i => i.RaiseEvent(e);

			if (e.DispatchOrder == EventDispatchOrder.FromHead)
			{
				Loop(action);
			}
			else
			{
				LoopReversed(action);
			}
		}

		private void Loop(Action<EventListener> action)
		{
			EventListener[] copy = Listeners.ToArray();
			copy.Length.Repeat(i => action(copy[i]));
		}

		private void LoopReversed(Action<EventListener> action)
		{
			EventListener[] copy = Listeners.ToArray();
			copy.Length.Repeat(i => action(copy[copy.Length - 1 - i]));
		}
	}
}
