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
            get { return Player != null ? Player.Game : null; }
		}

		/// <summary>
		/// Get the player to whom this controller belongs
		/// </summary>
		public Player Player
		{
			get; internal set;
		}

		protected BaseController()
		{
			InitializeMessaging();
		}
    }
}
