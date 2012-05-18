using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace TouhouSpring.Style
{
	interface IStyleElement
	{
		IStyleContainer Parent { get; }
		IEnumerable<IBindingProvider> BindingProviders { get; }
		void Initialize();
		void Apply();
	}
}
