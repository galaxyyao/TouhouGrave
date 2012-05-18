using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
	class ScriptRunner : ICommandlet
	{
		public string Tag
		{
			get { return "script.run"; }
		}

		public void Execute(params string[] parms)
		{
			if (parms.Length < 1)
			{
				Console.WriteLine("Usage: Script.Run path");
				Console.WriteLine("  path:\tPath of the script file.");
				Console.WriteLine("");
				return;
			}

			string path = parms[0];
			Program.NextInput = new StreamReader(path);
		}
	}
}
