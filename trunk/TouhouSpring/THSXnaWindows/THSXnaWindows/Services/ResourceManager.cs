using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace TouhouSpring.Services
{
	partial class ResourceManager : GameService
	{
		public interface IManager
		{
			bool IsRelevant(Type resourceType);
			T Load<T>(string uri);
			void Unload(object resource);
			void Dispose();
		}

		private ContentManager m_defaultContentMgr;
		private IManager[] m_managers;

		public T Acquire<T>(string uri)
		{
			foreach (var manager in m_managers)
			{
				if (manager.IsRelevant(typeof(T)))
				{
					return manager.Load<T>(uri);
				}
			}
			return m_defaultContentMgr.Load<T>(uri);
		}

		public void Release(object resourceObj)
		{
			var type = resourceObj.GetType();
			foreach (var manager in m_managers)
			{
				if (manager.IsRelevant(type))
				{
					manager.Unload(resourceObj);
					return;
				}
			}
		}

		public override void Startup()
		{
			List<IManager> managers = new List<IManager>();
			foreach (var type in AssemblyReflection.GetTypesImplements<IManager>())
			{
				managers.Add(type.Assembly.CreateInstance(type.FullName) as IManager);
			}
			m_managers = managers.ToArray();
			m_defaultContentMgr = new ContentManager(GameApp.Instance.Content.ServiceProvider, GameApp.Instance.Content.RootDirectory);
		}

		public override void Shutdown()
		{
			foreach (var manager in m_managers)
			{
				manager.Dispose();
			}
			m_defaultContentMgr.Dispose();
		}
	}
}
