using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
    /// <summary>
    /// Represent a decision maker for the player.
    /// </summary>
    public partial class BaseController
    {
        private bool m_sendNotifications;

        /// <summary>
        /// Get the Game
        /// </summary>
        public Game Game
        {
            get; internal set;
        }

        protected BaseController()
            : this(true, false)
        { }

        protected BaseController(bool sendNotifications, bool syncMode)
        {
            m_sendNotifications = sendNotifications;

            InitializeMessaging(syncMode);
        }

        public virtual int GetRandomSeed() { return -1; }
    }
}
