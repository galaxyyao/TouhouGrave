using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.UI
{
	class MouseEventRelay : EventRelay
	{
		public bool SetHandledAfterRelay
		{
			get; set;
		}

		public MouseEventRelay(EventListener target) : base(target)
		{ }

		public override void RaiseEvent<TEventArgs>(TEventArgs e)
		{
			if (e is MouseEventArgs)
			{
				Target.RaiseEvent(e);

				if (SetHandledAfterRelay)
				{
					(e as MouseEventArgs).SetHandled();
				}
			}
		}
	}
}
