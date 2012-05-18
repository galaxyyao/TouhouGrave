using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
	interface ICommandlet
	{
		string Tag { get; }
		void Execute(params string[] parameters);
	}
}
