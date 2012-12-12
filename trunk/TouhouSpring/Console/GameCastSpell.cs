using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
	class GameCastSpell : ICommandlet
	{
		public string Tag
		{
			get { return "game.cast"; }
		}

		public void Execute(params string[] parms)
		{
			if (!(Program.ActiveInteraction is Interactions.TacticalPhase))
			{
				Console.WriteLine("ERROR: This command can't be invoked now.");
				Console.WriteLine("");
				return;
			}

			int spellNo;
			if (parms.Length != 1 || !int.TryParse(parms[0], out spellNo))
			{
				Console.WriteLine("Usage: Game.Cast spell");
				Console.WriteLine("  Cast the spell on the specified card.");
				Console.WriteLine("  spell:\tOrdinal number of spells of cards in battlefield.");
                Console.WriteLine("Type Game.CastFrom to get the list of castable spells.");
                Console.WriteLine("");
				return;
			}

            var io = Program.ActiveInteraction as Interactions.TacticalPhase;
            var cards = io.ComputeCastFromSet();
            Behaviors.ICastableSpell spellToCast = null;

            int counter = 0;
            for (int i = 0; i < cards.Count; ++i)
            {
                var card = cards[i];
                foreach (var spell in card.Spells)
                {
                    if (++counter == spellNo)
                    {
                        spellToCast = spell;
                        break;
                    }
                }
            }

            if (spellToCast == null)
            {
                Console.WriteLine("ERROR: Can't find spell.");
                Console.WriteLine("Type Game.CastFrom to get the list of castable spells.");
                Console.WriteLine("");
                return;
            }

            while (true)
            {
                Console.Write("?? Cast {0} (Y/N) ? ", spellToCast.Model.Name);
                string choice = Console.ReadLine().ToLower();
                if (choice == "y" || choice == "yes")
                {
                    ((Interactions.TacticalPhase)Program.ActiveInteraction).Respond(spellToCast);
                    Program.ActiveInteraction = null;
                    break;
                }
                else if (choice == "n" || choice == "no")
                {
                    break;
                }
            }
		}
	}
}
