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

        public Graphics.TextRenderer TextRenderer
        {
            get; private set;
        }

		public RenderEventArgs() : base(EventDispatchOrder.FromHead)
		{
            RenderManager = GameApp.Service<Graphics.RenderManager>();
            TextRenderer = GameApp.Service<Graphics.TextRenderer>();
		}
	}
}
