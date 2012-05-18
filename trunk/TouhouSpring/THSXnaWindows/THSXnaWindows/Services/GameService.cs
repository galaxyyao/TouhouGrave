using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Services
{
	public class GameService : IService
	{
		public virtual void Startup() { }
		public virtual void Shutdown() { }
		public virtual void Update(float deltaTime) { }
		public virtual void Render() { }
	}

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class UpdateDependencyAttribute : DependencyAttribute
	{
		public static new string Category
		{
			get { return "Update"; }
		}

		public UpdateDependencyAttribute(Type type)
			: base(Category, type)
		{ }
	}

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class RenderDependencyAttribute : DependencyAttribute
	{
		public static new string Category
		{
			get { return "Render"; }
		}

		public RenderDependencyAttribute(Type type)
			: base(Category, type)
		{ }
	}
}
