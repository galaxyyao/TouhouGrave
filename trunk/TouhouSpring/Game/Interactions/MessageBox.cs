using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TouhouSpring.Interactions
{
	public class MessageBox : BaseInteraction
	{
		[Flags]
		public enum Button
		{
			OK = 0x01,
			Cancel = 0x02,
			Yes = 0x04,
			No = 0x08,
		}

		public BaseController Controller
		{
			get; private set;
		}

		public string Text
		{
			get; private set;
		}

		public Button Buttons
		{
			get; private set;
		}

		public Button Run()
		{
			return NotifyAndWait<Button>(Controller);
		}

		public void Respond(Button button)
		{
			if ((Buttons & button) == 0)
			{
				throw new ArgumentException(String.Format("Button '{0}' shall not be selected.", button));
			}

			RespondBack(Controller, button);
		}

		public MessageBox(BaseController controller, string text, Button buttons)
		{
			Debug.Assert(controller != null && text != null);
			Controller = controller;
			Text = text;
			Buttons = buttons;
		}
	}
}
