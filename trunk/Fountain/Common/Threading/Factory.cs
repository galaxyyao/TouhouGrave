using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Threading
{
	public interface IFactory
	{
		IEvent NewEvent(bool autoReset, bool singaled);
		IMutex NewMutex();
	}

	public class Factory
	{
		private static IFactory s_instance;

		public static IFactory Instance
		{
			get
			{
				if (s_instance == null)
				{
					bool implementFound = false;
					foreach (Type t in AssemblyReflection.GetTypesImplements<IFactory>())
					{
						implementFound = true;
						s_instance = (IFactory)t.Assembly.CreateInstance(t.FullName);
						break;
					}
					if (!implementFound)
					{
						throw new ApplicationException(String.Format("Can't find one {0} implement.", typeof(IFactory).FullName));
					}
				}
				return s_instance;
			}
		}
	}
}
