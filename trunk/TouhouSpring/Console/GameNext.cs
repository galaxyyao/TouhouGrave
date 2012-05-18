using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
	class GameNext : ICommandlet
	{
		public string Tag
		{
			get { return "game.next"; }
		}

		public void Execute(params string[] parms)
		{
			if (Program.ActiveInteraction is Interactions.SelectCards)
			{
				((Interactions.SelectCards)Program.ActiveInteraction).Respond(Indexable.Empty<BaseCard>());
			}
			else if (Program.ActiveInteraction is Interactions.BlockPhase)
			{
				var blockers = (from b in GameBlock.s_blockers select b.ToIndexable()).ToArray().ToIndexable();
				GameBlock.s_blockers = null;
				((Interactions.BlockPhase)Program.ActiveInteraction).Respond(blockers);
			}
			else
			{
				Console.WriteLine("ERROR: This command can't be invoked now.");
				Console.WriteLine("");
			}

			Program.ActiveInteraction = null;
		}
	}
}
