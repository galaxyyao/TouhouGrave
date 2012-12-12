using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
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

        public Player Player
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
			return NotifyAndWait<Button>(Player.Controller);
		}

		public void Respond(Button button)
		{
			if ((Buttons & button) == 0)
			{
				throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, "Button '{0}' shall not be selected.", button));
			}

			RespondBack(Player.Controller, button);
		}

		public MessageBox(Player player, string text, Button buttons)
		{
			Debug.Assert(player != null && text != null);
			Player = player;
			Text = text;
			Buttons = buttons;
		}
	}
}
