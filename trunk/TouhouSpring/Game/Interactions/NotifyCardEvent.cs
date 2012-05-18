using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TouhouSpring.Interactions
{
	public class NotifyCardEvent : NotifyOnly
	{
		public BaseCard Card
		{
			get; private set;
		}

		public string Message
		{
			get; private set;
		}

		internal NotifyCardEvent(BaseController controller, string notification, BaseCard card)
			: this(controller, notification, card, null)
		{ }

		internal NotifyCardEvent(BaseController controller, string notification, BaseCard card, string message)
			: base(controller, notification)
		{
			Debug.Assert(card != null);
			Card = card;
			Message = message;
		}
	}
}
