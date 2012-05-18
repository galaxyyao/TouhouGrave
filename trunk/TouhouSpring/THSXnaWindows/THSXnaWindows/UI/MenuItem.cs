using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.UI
{
	class MenuItem : MouseTrackedControl
	{
		public Label Label
		{
			get; private set;
		}

		public MenuItem()
		{
			Label = new Label
			{
				Dispatcher = this
			};
		}
	}
}
