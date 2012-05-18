using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.UI
{
	public class EventRelay : EventListener
	{
		public EventListener Target
		{
			get; private set;
		}

		public EventRelay(EventListener target)
		{
			if (target == null)
			{
				throw new ArgumentNullException("target");
			}

			Target = target;
		}

		public override void RaiseEvent<TEventArgs>(TEventArgs e)
		{
			Target.RaiseEvent(e);
		}
	}
}
