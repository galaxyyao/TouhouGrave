using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
	public interface ICastableSpell : IBehavior
	{
		void Run(Commands.CastSpell command);
	}
}
