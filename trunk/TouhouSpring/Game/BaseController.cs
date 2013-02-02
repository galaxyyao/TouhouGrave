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

        internal void BackupGame(Game.BackupPoint backupPoint)
        {
            OnGameBackup(backupPoint);
        }

        protected BaseController()
            : this(false)
        { }

        protected BaseController(bool syncMode)
        {
            InitializeMessaging(syncMode);
        }

        protected virtual void OnGameBackup(Game.BackupPoint backupPoint) { }
    }
}
