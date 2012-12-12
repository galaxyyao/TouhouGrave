using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
	public partial class BaseController
	{
		/// <summary>
		/// Game calls this method when the phase is changed among two of the five player phases.
		/// </summary>
		/// <param name="player">Whose turn</param>
		internal void InternalOnPlayerPhaseChanged(Player player)
		{
			Debug.Assert(player != null);

			if (player == Player)
			{
				new Interactions.NotifyControllerEvent(this, "OnPlayerPhaseChanged", player, Game.CurrentPhase).Run();
			}
			else
			{
			}
		}
	}
}
