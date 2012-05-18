using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Triggers
{
	/// <summary>
	/// Triggered before a spell is casted by a card.
	/// </summary>
	[TriggerFactory.Id("precast")]
	public class PreCastContext : CancelableTriggerContext
	{
		internal PreCastContext(Game game)
			: base(game)
		{

		}
	}
}
