using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.UI
{
	class RenderEventArgs : EventArgs
	{
		public Graphics.RenderManager RenderManager
		{
			get; private set;
		}

		public RenderEventArgs(Graphics.RenderManager renderMgr)
			: base(EventDispatchOrder.FromHead)
		{
			if (renderMgr == null)
			{
				throw new ArgumentNullException("renderMgr");
			}

			RenderManager = renderMgr;
		}
	}
}
