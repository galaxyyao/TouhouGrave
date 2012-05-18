using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace TouhouSpring.Style
{
	interface IStyleContainer : IStyleElement
	{
		string Id { get; }
		Dictionary<string, IStyleContainer> ChildIds { get; }
		Rectangle Bounds { get; }
		XElement Definition { get; }
		UI.EventDispatcher Target { get; }
	}
}
