using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Style
{
	interface IBindingProvider
	{
		bool TryGetValue(string id, out string replacement);
	}
}
