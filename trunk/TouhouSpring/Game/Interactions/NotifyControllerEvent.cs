using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TouhouSpring.Interactions
{
	public class NotifyControllerEvent : NotifyOnly
	{
		public Player Player
		{
			get;
			private set;
		}

		public string Message
		{
			get;
			private set;
		}

		internal NotifyControllerEvent(BaseController controller, string notification, Player player)
			: this(controller, notification, player, null)
		{ }

		internal NotifyControllerEvent(BaseController controller, string notification, Player player, string message)
			: base(controller, notification)
		{
			Debug.Assert(player != null);
			Player = player;
			Message = message;
		}
	}
}
