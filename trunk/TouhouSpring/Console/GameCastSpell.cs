﻿using System;
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

            if (spellNo >= io.CastSpellCandidates.Count)
            {
                Console.WriteLine("ERROR: Can't find spell.");
                Console.WriteLine("Type Game.CastFrom to get the list of castable spells.");
                Console.WriteLine("");
                return;
            }

            while (true)
            {
                Console.Write("?? Cast {0} (Y/N) ? ", io.CastSpellCandidates[spellNo].Model.Name);
                string choice = Console.ReadLine().ToLower();
                if (choice == "y" || choice == "yes")
                {
                    ((Interactions.TacticalPhase)Program.ActiveInteraction).RespondCast(io.CastSpellCandidates[spellNo]);
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
