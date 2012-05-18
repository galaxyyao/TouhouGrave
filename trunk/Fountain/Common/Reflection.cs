using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TouhouSpring
{
	public static class Reflection
	{
		public static T GetAttribute<T>(this Type type)
			where T : Attribute
		{
			return type.GetAttribute<T>(false);
		}

		public static T GetAttribute<T>(this Type type, bool inherit)
			where T : Attribute
		{
			var attrs = type.GetCustomAttributes(typeof(T), inherit);
			return attrs.Length > 0 ? attrs[0] as T : null;
		}

		public static bool HasAttribute<T>(this Type type)
			where T : Attribute
		{
			return HasAttribute<T>(type, false);
		}

		public static bool HasAttribute<T>(this Type type, bool inherit)
			where T : Attribute
		{
			return type.GetCustomAttributes(typeof(T), inherit).Length > 0;
		}

		public static bool HasInterface<T>(this Type type)
			where T : class
		{
			return type.GetInterfaces().Contains(typeof(T));
		}

		public static bool IsSubclassOf<T>(this Type type)
			where T : class
		{
			return type.IsSubclassOf(typeof(T));
		}
	}
}
