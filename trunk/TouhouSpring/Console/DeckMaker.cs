using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
	class DeckMaker
	{
		static public Dictionary<string, Deck> s_decks = new Dictionary<string, Deck>();
		static public Deck s_currentModifingDeck;

		public class NewDeck : ICommandlet
		{
			public string Tag
			{
				get { return "deck.new"; }
			}

			public void Execute(params string[] parms)
			{
				if (parms.Length < 1)
				{
					Console.WriteLine("Usage: Deck.New name");
					Console.WriteLine("  name:\tName of the deck being created.");
					Console.WriteLine("");
					return;
				}

				string deckName = parms[0];

				Deck deck = new Deck(deckName);
				if (s_decks.ContainsKey(deckName))
				{
					s_decks[deckName] = deck;
				}
				else
				{
					s_decks.Add(deckName, deck);
				}
				s_currentModifingDeck = deck;
			}
		}

		public class AddCard : ICommandlet
		{
			public string Tag
			{
				get { return "deck.add"; }
			}

			public void Execute(params string[] parms)
			{
				if (parms.Length < 1)
				{
					Console.WriteLine("Usage: Deck.Add cardid");
					Console.WriteLine("  cardid:\tId of the card to be added to the deck.");
					Console.WriteLine("");
					return;
				}
				else if (s_currentModifingDeck == null)
				{
					Console.Error.WriteLine("ERROR: No deck is selected for modification.");
					return;
				}

				string cardId = parms[0];

				s_currentModifingDeck.Add(Program.CardFactory.GetCardModel(cardId));
			}
		}
	}
}
