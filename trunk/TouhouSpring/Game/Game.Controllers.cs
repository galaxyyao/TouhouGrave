using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
	public partial class Game
	{
		private BaseController[] m_controllers;

		/// <summary>
		/// Give the current player's Controller object.
		/// </summary>
		public BaseController PlayerController
		{
			get
			{
				if (!InPlayerPhases)
				{
					throw new InvalidOperationException("ActingController can only be evaluated in one of the five player phases.");
				}
				return m_controllers[m_actingPlayer];
			}
		}

		/// <summary>
		/// Give the opponent player's Controller object.
		/// TODO: support game among more than 2 players
		/// </summary>
		public BaseController OpponentController
		{
			get
			{
				if (!InPlayerPhases)
				{
					throw new InvalidOperationException("OpponentController can only be evaluated in one of the five player phases.");
				}
				return m_controllers[1 - m_actingPlayer];
			}
		}

		/// <summary>
		/// Return a collection of all BaseController objects.
		/// </summary>
		public IIndexable<BaseController> Controllers
		{
			get { return m_controllers.ToIndexable(); }
		}
	}
}
