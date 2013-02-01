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
        /// <summary>
        /// Get the Game
        /// </summary>
        public Game Game
        {
            get; internal set;
        }

        protected BaseController()
            : this(false)
        { }

        protected BaseController(bool syncMode)
        {
            InitializeMessaging(syncMode);
        }
    }
}
