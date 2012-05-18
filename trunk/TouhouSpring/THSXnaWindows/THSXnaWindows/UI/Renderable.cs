using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.UI
{
	interface IRenderable
	{
		void OnRender(RenderEventArgs e);
	}

	class Renderable : EventListener, IEventListener<RenderEventArgs>
	{
		private IRenderable TypedDispatcher
		{
			get { return Dispatcher as IRenderable; }
		}

		public Renderable(EventDispatcher dispatcher)
		{
			if (dispatcher == null)
			{
				throw new ArgumentNullException("dispatcher");
			}
			else if (!(dispatcher is IRenderable))
			{
				throw new ArgumentException("Dispatcher doesn't implement IRenderable.");
			}

			base.Dispatcher = dispatcher;
		}

		public void RaiseEvent(RenderEventArgs e)
		{
			TypedDispatcher.OnRender(e);
		}
	}
}
