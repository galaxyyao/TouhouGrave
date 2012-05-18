using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.UI
{
	class Page : EventDispatcher, IResourceContainer
	{
		private ResourceContainer m_resourceContainer = new ResourceContainer();

		public Style.PageStyle Style
		{
			get; private set;
		}

		public float Width
		{
			get { return GameApp.Service<Services.UIManager>().ViewportWidth; }
		}

		public float Height
		{
			get { return GameApp.Service<Services.UIManager>().ViewportHeight; }
		}

		public Page(Style.PageStyle style)
		{
			if (style == null)
			{
				throw new ArgumentNullException("style");
			}

			Style = style;
		}

		public void DisposeResources()
		{
			m_resourceContainer.ReleaseAll();
		}

		#region IResourceContainer implementation

		void IResourceContainer.Register(object resource)
		{
			m_resourceContainer.Register(resource);
		}

		void IResourceContainer.Release(object resource)
		{
			m_resourceContainer.Release(resource);
		}

		#endregion
	}
}
