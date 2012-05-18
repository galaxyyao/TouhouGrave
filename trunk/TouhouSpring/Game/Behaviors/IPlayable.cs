using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
	public interface IPlayable : IBehavior
	{
        bool IsPlayable(Game game);
	}
}
