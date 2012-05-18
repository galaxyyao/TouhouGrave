using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Services
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class DependencyAttribute : Attribute
	{
		public string Category
		{
			get; private set;
		}

		public Type Precedent
		{
			get; private set;
		}

		public DependencyAttribute(string category, Type type)
		{
			if (category == null)
			{
				throw new ArgumentNullException("category");
			}
			else if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			else if (!type.HasInterface<IService>())
			{
				throw new IncompleteTypeDefinitionException(typeof(IService));
			}

			Category = category;
			Precedent = type;
		}
	}

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class LifetimeDependencyAttribute : DependencyAttribute
	{
		public static new string Category
		{
			get { return "Lifetime"; }
		}

		public LifetimeDependencyAttribute(Type type)
			: base(Category, type)
		{ }
	}
}
