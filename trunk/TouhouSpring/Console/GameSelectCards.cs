using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
	class GameSelectCards : ICommandlet
	{
		public string Tag
		{
			get { return "game.select"; }
		}

		public void Execute(params string[] args)
		{
			if (!(Program.ActiveInteraction is Interactions.SelectCards))
			{
				Console.WriteLine("ERROR: This command can only be invoked when a SelectCards message is requested.");
				Console.WriteLine("");
				return;
			}

			var selectCards = (Interactions.SelectCards)Program.ActiveInteraction;

			if (selectCards.Mode == Interactions.SelectCards.SelectMode.Single && args.Length > 1)
			{
				Console.WriteLine("ERROR: Only one card can be selected.");
				Console.WriteLine("");
				return;
			}

            int tmp;
            if (args.Any(i => !int.TryParse(i, out tmp)))
            {
                Console.WriteLine("Usage: Game.Select [{0}]", selectCards.Mode == Interactions.SelectCards.SelectMode.Single ? "card" : "card1 [card2...]");
                Console.WriteLine("  Select {0} as required by the game.", selectCards.Mode == Interactions.SelectCards.SelectMode.Single ? "card" : "cards");
                Console.WriteLine("  {0}:\tOrdinal number of all selectable cards.", selectCards.Mode == Interactions.SelectCards.SelectMode.Single ? "card" : "card1, card2...");
                Console.WriteLine("Type Game.SelectFrom to get the list of selectable cards.");
                Console.WriteLine("");
                return;
            }

            BaseCard[] cards = new BaseCard[args.Length];
            for (int i = 0; i < args.Length; ++i)
            {
                cards[i] = selectCards.FromSet[int.Parse(args[i]) - 1];
            }

            while (true)
            {
                Console.Write("?? Select {0} (Y/N) ? ",
                              cards.Length == 0 ? "nothing" : String.Join(" ", (from c in cards select c.Print()).ToArray()));
                string choice = Console.ReadLine().ToLower();
                if (choice == "y" || choice == "yes")
                {
                    selectCards.Respond(cards.ToIndexable());
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
