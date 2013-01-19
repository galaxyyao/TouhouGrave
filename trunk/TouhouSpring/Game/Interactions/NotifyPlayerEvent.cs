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

        internal NotifyPlayerEvent(Game game, string notification, Player player)
            : this(game, notification, player, null)
        { }

        internal NotifyPlayerEvent(Game game, string notification, Player player, string message)
            : base(game, notification)
        {
            Debug.Assert(player != null);
            Player = player;
            Message = message;
        }
    }
}
