using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Interactions
{
	public class NotifyGameEvent : NotifyOnly
	{
    	public string Message
    	{
    		get; private set;
    	}

		internal NotifyGameEvent(BaseController controller, string notification, string message)
			: base(controller, notification)
		{
			Message = message;
		}
	}
}
