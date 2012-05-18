using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Services
{
	public class Container<TService> where TService : class, IService
	{
		private enum ServiceState
		{
			Stopped,
			StartingUp,
			Running,
			ShuttingDown,
		};

		private class Entry
		{
			public TService m_service;
			public Dictionary<string, List<Type>> m_dependencies = new Dictionary<string, List<Type>>();
			public ServiceState m_state = ServiceState.Stopped;

			public Entry(TService srv)
			{
				m_service = srv;
			}
		};

		private List<Type> m_serviceTypes = new List<Type>();
		private Dictionary<Type, Entry> m_services = new Dictionary<Type, Entry>();
		private Dictionary<string, List<Type>> m_sortedLists = new Dictionary<string, List<Type>>();

		public Container()
		{
			IEnumerable<Type> relevantTypes = typeof(TService).IsInterface
											  ? AssemblyReflection.GetTypesImplements<TService>()
											  : AssemblyReflection.GetTypesDerivedFrom<TService>();
			m_serviceTypes = relevantTypes.ToList();

			// instantiate services
			foreach (Type t in relevantTypes)
			{
				m_services.Add(t, new Entry(t.Assembly.CreateInstance(t.FullName) as TService));
			}

			// resolve dependencies for each category
			foreach (var kvp in m_services)
			{
				var srvType = kvp.Key;
				var srvEntry = kvp.Value;

				foreach (var attr in srvType.GetCustomAttributes(typeof(DependencyAttribute), true))
				{
					var depAttr = attr as DependencyAttribute;
					if (!srvEntry.m_dependencies.ContainsKey(depAttr.Category))
					{
						// register the category in sorted list
						if (!m_sortedLists.ContainsKey(depAttr.Category))
						{
							m_sortedLists.Add(depAttr.Category, new List<Type>());
						}
						srvEntry.m_dependencies.Add(depAttr.Category, new List<Type>());
					}
					srvEntry.m_dependencies[depAttr.Category].Add(depAttr.Precedent);
				}
			}

			// sort services of each category
			foreach (var kvp in m_sortedLists)
			{
				Stack<Type> visitStack = new Stack<Type>();
				m_services.Keys.ForEach(srvType => DependencySort(srvType, visitStack, kvp));
			}
		}

		public T Get<T>() where T : class, TService
		{
			Entry e;
			if (!m_services.TryGetValue(typeof(T), out e))
			{
				throw new ArgumentException(String.Format("Unknown service type accessed: '{0}'.", typeof(T).Name));
			}

			if (e.m_state != ServiceState.Running)
			{
				throw new InvalidOperationException(String.Format("Service of type '{0}' is not running. Current state: {1}", typeof(T).Name, e.m_state));
			}

			return e.m_service as T;
		}

		public void Startup()
		{
			Traverse(LifetimeDependencyAttribute.Category, false, srv => {
				Entry e = m_services[srv.GetType()];
				e.m_state = ServiceState.StartingUp;
				srv.Startup();
				e.m_state = ServiceState.Running;
			});
		}

		public void Shutdown()
		{
			Traverse(LifetimeDependencyAttribute.Category, true, srv => {
				Entry e = m_services[srv.GetType()];
				e.m_state = ServiceState.ShuttingDown;
				srv.Shutdown();
				e.m_state = ServiceState.Stopped;
			});
		}

		public void Traverse(string dependencyCategory, bool reversedOrder, Action<TService> action)
		{
			if (dependencyCategory == null)
			{
				throw new ArgumentNullException("dependencyCategory");
			}
			else if (action == null)
			{
				throw new ArgumentNullException("action");
			}

			List<Type> srvList;
			if (!m_sortedLists.TryGetValue(dependencyCategory, out srvList))
			{
				srvList = m_serviceTypes;
			}

			if (!reversedOrder)
			{
				for (int i = 0; i < srvList.Count; ++i)
				{
					action(m_services[srvList[i]].m_service);
				}
			}
			else
			{
				for (int i = srvList.Count - 1; i >= 0; --i)
				{
					action(m_services[srvList[i]].m_service);
				}
			}
		}

		private void DependencySort(Type srcType, Stack<Type> visitStack, KeyValuePair<string, List<Type>> sortedCategory)
		{
			if (visitStack.Contains(srcType))
			{
				throw new ApplicationException("Circular init order detected!");
			}

			Entry e = m_services[srcType];

			if (sortedCategory.Value.Contains(srcType))
			{
				// already sorted
				return;
			}

			visitStack.Push(srcType);

			List<Type> precedents;
			if (e.m_dependencies.TryGetValue(sortedCategory.Key, out precedents))
			{
				// sort precedents first
				precedents.ForEach(depType => DependencySort(depType, visitStack, sortedCategory));
			}

			sortedCategory.Value.Add(srcType);
			visitStack.Pop();
		}
	}
}
