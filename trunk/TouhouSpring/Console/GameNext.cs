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
			if (Program.ActiveInteraction is Interactions.TacticalPhase)
            {
                ((Interactions.TacticalPhase)Program.ActiveInteraction).RespondPass();
            }
            else if (Program.ActiveInteraction is Interactions.SelectCards)
			{
				((Interactions.SelectCards)Program.ActiveInteraction).Respond(Indexable.Empty<BaseCard>());
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
