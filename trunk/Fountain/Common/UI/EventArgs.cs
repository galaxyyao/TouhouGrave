using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.UI
{
	public enum EventDispatchOrder
	{
		FromHead,
		FromTail
	}

	public class EventArgs : System.EventArgs
	{
		public EventDispatchOrder DispatchOrder
		{
			get; private set;
		}

		public EventArgs() : this(EventDispatchOrder.FromHead)
		{ }

		public EventArgs(EventDispatchOrder dispatchOrder)
		{
			DispatchOrder = dispatchOrder;
		}
	}
}
