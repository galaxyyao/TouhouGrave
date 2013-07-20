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

    public class MessageBox : BaseInteraction, IQuickInteraction
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
            return NotifyAndWait<MessageBoxButtons>();
        }

        public void Respond(MessageBoxButtons button)
        {
            if ((Buttons & button) == 0)
            {
                throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, "Button '{0}' shall not be selected.", button));
            }

            RespondBack(button);
        }

        public MessageBox(Player player, string text, MessageBoxButtons buttons)
            : base(player.Game)
        {
            Debug.Assert(player != null && !String.IsNullOrEmpty(text));
            Player = player;
            Text = text;
            Buttons = buttons;
        }

        object IQuickInteraction.Run()
        {
            var result = Run();
            return result != MessageBoxButtons.Cancel ? (MessageBoxButtons?)result : null;
        }

        bool IQuickInteraction.HasCandidates()
        {
            return Buttons != 0;
        }
    }
}
