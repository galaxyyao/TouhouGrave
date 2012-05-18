using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
	class GameView : ICommandlet
	{
		public string Tag
		{
			get { return "game.view"; }
		}

		public void Execute(params string[] parms)
		{
			Console.WriteLine();
			foreach (var player in Program.TouhouSpringGame.Players)
			{
				Console.WriteLine("{1} {0}<{2}|{3}> {1}", player.Name, player == Program.TouhouSpringGame.PlayerPlayer ? "***" : "===", player.Health, player.Mana);
				Console.WriteLine(player.CardsOnHand.Aggregate("<Hand>", (str, card) => str + " " + card.Print()));
				Console.WriteLine(player.CardsOnBattlefield.Aggregate("<Battlefield>", (str, card) => str + " " + card.Print()));
				Console.WriteLine();
			}
		}
	}
}
