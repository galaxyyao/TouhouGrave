using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TouhouSpring.Interactions
{
	public class NotifyOnly : BaseInteraction
	{
		public BaseController Controller
		{
			get; private set;
		}

		public string Notification
		{
			get; private set;
		}

		public void Run()
		{
			var ret = NotifyAndWait<object>(Controller);
		}

		public void Respond()
		{
			RespondBack(Controller, string.Empty);
		}

		internal NotifyOnly(BaseController controller, string notification)
		{
			Debug.Assert(controller != null && notification != null);
			Controller = controller;
			Notification = notification;
		}
	}
}
