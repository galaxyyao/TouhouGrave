using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TouhouSpring.Interactions
{
    public class NotifyCardEvent : NotifyOnly
    {
        public CardInstance Card
        {
            get; private set;
        }

        public string Message
        {
            get; private set;
        }

        internal NotifyCardEvent(Game game, string notification, CardInstance card)
            : this(game, notification, card, null)
        { }

        internal NotifyCardEvent(Game game, string notification, CardInstance card, string message)
            : base(game, notification)
        {
            Debug.Assert(card != null);
            Card = card;
            Message = message;
        }
    }
}
