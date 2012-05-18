using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Triggers
{
	/// <summary>
	/// The base class for trigger context.
	/// </summary>
	public class TriggerContext
	{
		/// <summary>
		/// The current Game object
		/// </summary>
		public Game Game
		{
			get;
			private set;
		}

		protected TriggerContext(Game game)
		{
			if (game == null)
			{
				throw new ArgumentNullException("game");
			}

			Game = game;
		}
	}

	/// <summary>
	/// The base class for a cancellable trigger context.
	/// </summary>
	public class CancelableTriggerContext : TriggerContext
	{
		public bool Cancel
		{
			get; set;
		}

		public string Reason
		{
			get; set;
		}

		protected CancelableTriggerContext(Game game)
			: base(game)
		{
			Cancel = false;
			Reason = string.Empty;
		}
	}
}
