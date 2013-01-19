using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
	class GameStarter
	{
		static public List<GameStartupParameters> s_startupParams = new List<GameStartupParameters>();

		public class AddGamePlayer : ICommandlet
		{
			public string Tag
			{
				get { return "game.addplayer"; }
			}

			public void Execute(params string[] parms)
			{
				if (parms.Length < 2)
				{
					Console.WriteLine("Usage: Game.AddPlayer playername, deckname heroname");
					Console.WriteLine("  playername:\tName of the player.");
					Console.WriteLine("  deckname:\tName of the deck.");
					Console.WriteLine("");
					return;
				}

				string playerName = parms[0];
				string deckName = parms[1];

				var deck = DeckMaker.s_decks[deckName];

				GameStartupParameters gsp = new GameStartupParameters();
				gsp.m_profile = new Profile();
				gsp.m_profile.Name = playerName;
				gsp.m_profile.Decks.Add(deck);
				gsp.m_deck = deck;
				s_startupParams.Add(gsp);
			}
		}

		public class BeginGame : ICommandlet
		{
			public string Tag
			{
				get { return "game.begin"; }
			}

			public void Execute(params string[] parms)
			{
                Program.TouhouSpringGame = new Game(s_startupParams.ToIndexable(), new ConsoleController());
			}
		}
	}
}
