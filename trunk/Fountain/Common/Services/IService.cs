using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Services
{
	public interface IService
	{
		void Startup();
		void Shutdown();
	}
}
