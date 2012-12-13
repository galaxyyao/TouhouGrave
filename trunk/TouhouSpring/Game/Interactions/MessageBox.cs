using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;

namespace TouhouSpring.Interactions
{
    [Flags]
    public enum MessageBoxButtons
    {
        OK = 0x01,
        Cancel = 0x02,
        Yes = 0x04,
        No = 0x08,
    }

	public class MessageBox : BaseInteraction
	{
        public Player Player
        {
            get; private set;
        }

        public string Text
		{
			get; private set;
		}

        public MessageBoxButtons Buttons
        {
            get; private set;
        }

        public MessageBoxButtons Run()
		{
            return NotifyAndWait<MessageBoxButtons>(Player.Controller);
		}

        public void Respond(MessageBoxButtons button)
		{
			if ((Buttons & button) == 0)
			{
				throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, "Button '{0}' shall not be selected.", button));
			}

			RespondBack(Player.Controller, button);
		}

        public MessageBox(Player player, string text, MessageBoxButtons buttons)
		{
			Debug.Assert(player != null && text != null);
			Player = player;
			Text = text;
			Buttons = buttons;
		}
	}
}
