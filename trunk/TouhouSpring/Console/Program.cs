using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
	static class Program
	{
        private static PathUtils s_pathUtils = new PathUtils(
            Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location),
            Properties.Settings.Default.ContentRootDirectory));

		public static Game TouhouSpringGame
		{
			get; set;
		}

        public static CardFactory CardFactory
        {
            get; private set;
        }

		public static System.IO.StreamReader NextInput
		{
			get; set;
		}

		public static Interactions.BaseInteraction ActiveInteraction
		{
			get; set;
		}

        public static string Print(this BaseCard card)
        {
            return "|" + card.Model.Name + "|";
        }

		private static void Main(string[] args)
		{
			Console.WriteLine("\nWelcome to TouhouSpring Console.\n");

			Dictionary<string, ICommandlet> commandlets = new Dictionary<string,ICommandlet>();
			foreach (Type t in AssemblyReflection.GetTypesImplements<ICommandlet>())
			{
				ICommandlet cmd = t.Assembly.CreateInstance(t.FullName) as ICommandlet;
				if (commandlets.ContainsKey(cmd.Tag))
				{
					Console.Error.WriteLine("ERROR: Command '{0}' redefined.", cmd.Tag);
				}
				else
				{
					commandlets.Add(cmd.Tag, cmd);
				}
			}

            CardFactory = new CardFactory(s_pathUtils.ToDiskPath("TouhouSpring", "xml"));

			while (true)
			{
				Console.Write("? ");

				string userInput;
				if (NextInput != null)
				{
					userInput = NextInput.ReadLine();
					Console.WriteLine(userInput);
					if (NextInput.EndOfStream)
					{
						NextInput = null;
					}
				}
				else
				{
					userInput = Console.ReadLine();
				}

				string[] cmds = userInput.ToLower().Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
				foreach (var cmdStr in cmds)
				{
					string[] tokens = cmdStr.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

					ICommandlet cmd;
					if (tokens.Length == 0 || tokens[0].StartsWith("#"))
					{
						continue;
					}
					else if (tokens[0] == "exit")
					{
						return;
					}
					else if (tokens[0] == "help")
					{
						foreach (string tag in commandlets.Keys)
						{
							Console.WriteLine("  " + tag);
						}
						Console.WriteLine();
					}
					else if (commandlets.TryGetValue(tokens[0], out cmd))
					{
						string[] parms = new string[tokens.Length - 1];
						Array.Copy(tokens, 1, parms, 0, parms.Length);
						try
						{
							cmd.Execute(parms);
							ContinueGame();
						}
						catch (Exception e)
						{
							Console.Error.WriteLine("ERROR: {0}: {1}", e.GetType().FullName, e.Message);
						}
					}
					else
					{
						Console.Error.WriteLine("ERROR: Unknown command '{0}'.", tokens[0]);
					}
				}
			}
		}

		private static void ContinueGame()
		{
			if (TouhouSpringGame == null || ActiveInteraction != null)
			{
				return;
			}

			while (true)
			{
				foreach (var player in TouhouSpringGame.Players)
				{
					if (player.Controller.ProcessMessage())
					{
						return;
					}
				}
				System.Threading.Thread.Sleep(10);
			}
		}
	}
}
