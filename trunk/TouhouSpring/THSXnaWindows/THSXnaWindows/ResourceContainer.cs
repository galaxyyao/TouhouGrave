using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
	class ResourceContainer
	{
		private List<object> m_resources = new List<object>();

		public void Register(object resource)
		{
			if (resource == null)
			{
				throw new ArgumentNullException("resource");
			}

			if (m_resources.Contains(resource))
			{
				// release the reference
				GameApp.Service<Services.ResourceManager>().Release(resource);
			}
			else
			{
				m_resources.Add(resource);
			}
		}

		public void Release(object resource)
		{
			if (resource == null)
			{
				throw new ArgumentNullException("resource");
			}

			int index = m_resources.IndexOf(resource);
			
			if (index == -1)
			{
				throw new ArgumentException("Resource is not previously registered.");
			}

			m_resources.RemoveAt(index);
		}

		public void ReleaseAll()
		{
			m_resources.ForEach(res => GameApp.Service<Services.ResourceManager>().Release(res));
            m_resources.Clear();
		}
	}
}
