using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TouhouSpring.Interactions
{
    public class NotifyPlayerEvent : NotifyOnly
    {
        public Player Player
        {
            get; private set;
        }

        public string Message
        {
            get; private set;
        }

        internal NotifyPlayerEvent(BaseController controller, string notification, Player player)
            : this(controller, notification, player, null)
        { }

        internal NotifyPlayerEvent(BaseController controller, string notification, Player player, string message)
            : base(controller, notification)
        {
            Debug.Assert(player != null);
            Player = player;
            Message = message;
        }
    }
}
