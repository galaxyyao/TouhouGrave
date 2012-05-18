using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TouhouSpring
{
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
	public class AssemblyReflectableAttribute : Attribute
	{ }

	public class AssemblyReflection
	{
		private static Assembly[] s_gatheredAssemblies = null;
		public static Assembly[] GetGatheredAssemblies()
		{
			if (s_gatheredAssemblies == null)
			{
				List<string> reflectableAssemblies = new List<string>();

				// create an AssemblyChecker in an additional AppDomain
				AppDomain safeDomain = AppDomain.CreateDomain("safeDomain");
				var ac = (AssemblyChecker)safeDomain.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName, typeof(AssemblyChecker).FullName);
				ac.RootDirectory = Environment.CurrentDirectory;
				ac.MarkAttributeName = typeof(AssemblyReflectableAttribute).FullName;

				foreach (string fileName in FilesOfInterest())
				{
					string assemblyFullName;
					if (ac.CheckAssembly(fileName, out assemblyFullName))
						reflectableAssemblies.Add(assemblyFullName);
				}

				ac = null;
				AppDomain.Unload(safeDomain);

				s_gatheredAssemblies = new Assembly[reflectableAssemblies.Count];
				for (int i = 0; i < reflectableAssemblies.Count; ++i)
					s_gatheredAssemblies[i] = Assembly.Load(reflectableAssemblies[i]);
			}
			return s_gatheredAssemblies;
		}

		private static IEnumerable<string> FilesOfInterest()
		{
			foreach (string fileName in Directory.GetFiles(Environment.CurrentDirectory, "*.dll", SearchOption.AllDirectories))
				yield return fileName;
			yield return Assembly.GetEntryAssembly().Location;
		}

		public static IEnumerable<Type> GetTypesDerivedFrom<T>() where T : class
		{
			foreach (Assembly asm in GetGatheredAssemblies())
				foreach (Type type in asm.GetTypes())
                    if (type.IsSubclassOf<T>())
                        yield return type;
		}

		public static IEnumerable<Type> GetTypesImplements<T>() where T : class
		{
			foreach (Assembly asm in GetGatheredAssemblies())
				foreach (Type t in asm.GetTypes())
					if (t.HasInterface<T>())
						yield return t;
		}

		public static IEnumerable<Type> GetTypesWithAttribute<T>() where T : Attribute
		{
			return GetTypesWithAttribute<T>(false);
		}

		public static IEnumerable<Type> GetTypesWithAttribute<T>(bool inherit) where T : Attribute
		{
			foreach (Assembly asm in GetGatheredAssemblies())
				foreach (Type t in asm.GetTypes())
					if (t.IsDefined(typeof(T), inherit))
						yield return t;
		}

		private class AssemblyChecker: MarshalByRefObject
		{
			public string RootDirectory { get; set; }
			public string MarkAttributeName { get; set; }

			public AssemblyChecker()
			{
				AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += new ResolveEventHandler(OnReflectionOnlyResolve);
			}

			public bool CheckAssembly(string path, out string fullName)
			{
				try
				{
					Assembly asm = Assembly.ReflectionOnlyLoadFrom(path);
					fullName = asm.FullName;
					return CustomAttributeData.GetCustomAttributes(asm)
						.Any(x => x.Constructor.ReflectedType.FullName == MarkAttributeName);
				}
				catch (BadImageFormatException)
				{
					// not a CLR assembly: silent
					fullName = "";
					return false;
				}
			}

			private Assembly OnReflectionOnlyResolve(object sender, ResolveEventArgs e)
			{
				Assembly loadedAssembly = AppDomain.CurrentDomain.ReflectionOnlyGetAssemblies()
					.FirstOrDefault(asm => string.Equals(asm.FullName, e.Name, StringComparison.OrdinalIgnoreCase));

				if (loadedAssembly != null)
					return loadedAssembly;

				AssemblyName assemblyName = new AssemblyName(e.Name);
				string dependentAssemblyFilename = Path.Combine(RootDirectory, assemblyName.Name + ".dll");
				return File.Exists(dependentAssemblyFilename)
					   ? Assembly.ReflectionOnlyLoadFrom(dependentAssemblyFilename)
					   : Assembly.ReflectionOnlyLoad(e.Name);
			}
		};
	}
}
