using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
	public interface ICastableSpell : IBehavior
	{
		bool Cast(Game game, out string reason);
	}
}
