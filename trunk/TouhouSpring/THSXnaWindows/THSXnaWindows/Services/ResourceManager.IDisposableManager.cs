using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace TouhouSpring.Services
{
	partial class ResourceManager
	{
		private class IDisposableManager : ContentManager, IManager
		{
			private class ResourceEntry
			{
				public string m_assetName;
				public IDisposable m_disposable;
				public int m_refCount;
			}
			private List<ResourceEntry> m_resources = new List<ResourceEntry>();

			public IDisposableManager()
				: base(GameApp.Instance.Content.ServiceProvider, GameApp.Instance.Content.RootDirectory)
			{ }

			public bool IsRelevant(Type type)
			{
				return type.HasInterface<IDisposable>();
			}

			public override T Load<T>(string uri)
			{
                int resIndex = m_resources.FindIndex(e => e.m_assetName == uri);
                if (resIndex != -1)
                {
                    ++m_resources[resIndex].m_refCount;

                    object ret = null;
                    for (int i = resIndex; i < m_resources.Count; ++i)
                    {
                        if (m_resources[i].m_assetName != uri)
                        {
                            break;
                        }
                        else if (m_resources[i].m_disposable.GetType() == typeof(T))
                        {
                            ret = m_resources[i].m_disposable;
                        }
                    }

                    return (T)ret;
                }
				else
				{
					return ReadAsset<T>(uri, disposable =>
					{
						m_resources.Add(new ResourceEntry { m_assetName = uri, m_disposable = disposable, m_refCount = 1 });
					});
				}
			}

			protected override void Dispose(bool disposing)
			{
				if (disposing)
				{
					m_resources.ForEach(e => e.m_disposable.Dispose());
				}
			}

			public void Unload(object resourceObject)
			{
				if (resourceObject == null)
				{
					throw new ArgumentNullException("resourceObject");
				}

				var disposable = resourceObject as IDisposable;
				int resIndex = m_resources.FindIndex(e => e.m_disposable == disposable);
				if (resIndex == -1)
				{
					throw new ArgumentException("Asset is not loaded or is unnecessary to unload.");
				}

                for (int i = resIndex - 1; i >= 0; --i)
                {
                    if (m_resources[i].m_assetName != m_resources[i].m_assetName)
                    {
                        resIndex = i + 1;
                        break;
                    }
                }

                if (--m_resources[resIndex].m_refCount == 0)
                {
                    var assetName = m_resources[resIndex].m_assetName;
                    while (resIndex < m_resources.Count && m_resources[resIndex].m_assetName == assetName)
                    {
                        m_resources[resIndex].m_disposable.Dispose();
                        m_resources.RemoveAt(resIndex);
                    }
                }
			}
		}
	}
}
