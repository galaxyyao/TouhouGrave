using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TouhouSpring.Interactions
{
    public class NotifyOnly : BaseInteraction
    {
        public string Notification
        {
            get; private set;
        }

        public void Run()
        {
            NotifyAndWait<object>();
        }

        public void Respond()
        {
            RespondBack(string.Empty);
        }

        internal NotifyOnly(Game game, string notification)
            : base(game)
        {
            Debug.Assert(!String.IsNullOrEmpty(notification));
            Notification = notification;
        }
    }
}
