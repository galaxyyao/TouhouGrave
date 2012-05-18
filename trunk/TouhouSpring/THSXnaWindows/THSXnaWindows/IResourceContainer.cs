using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
	interface IResourceContainer
	{
		void Register(object resource);
		void Release(object resource);
	}
}
