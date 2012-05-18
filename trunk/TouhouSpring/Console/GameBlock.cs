using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
	class GameBlock
	{
		public static List<List<BaseCard>> s_blockers;

		public class Declarator : ICommandlet
		{
			public string Tag
			{
				get { return "game.block"; }
			}

			public void Execute(params string[] parms)
			{
				if (!(Program.ActiveInteraction is Interactions.BlockPhase))
				{
					Console.WriteLine("ERROR: This command can only be invoked in Block phase.");
					Console.WriteLine("");
					return;
				}

				int tmp;
				if ((parms.Length != 0 && parms.Length != 2) || parms.Any(i => !int.TryParse(i, out tmp)))
				{
					Console.WriteLine("Usage: Game.Block [blocker attacker]");
					Console.WriteLine("  Blocker-attacker pairs will be printed after this command.");
					Console.WriteLine("  blocker:\tOrdinal number of card in player's battlefield to");
					Console.WriteLine("                  \tbe declared as the blocker.");
					Console.WriteLine("  attacker:\tOrdinal number of card in opponent's attackers to");
					Console.WriteLine("                  \tbe declared as the blockee.");
					Console.WriteLine("");
					return;
				}

				if (parms.Length > 0)
				{
					int blockerIndex = int.Parse(parms[0]) - 1;
					int attackerIndex = int.Parse(parms[1]) - 1;

					var blocker = Program.TouhouSpringGame.OpponentPlayer.CardsOnBattlefield.ElementAt(blockerIndex);
					if (!blocker.Behaviors.Has<Behaviors.Warrior>())
					{
						Console.WriteLine("ERROR: {0} is not a warrior.", blocker.Model.Name);
						Console.WriteLine("");
						return;
					}

					if (s_blockers[attackerIndex].Contains(blocker))
					{
						s_blockers[attackerIndex].Remove(blocker);
					}
					else
					{
						s_blockers[attackerIndex].Add(blocker);

						// remove the blocker from other attackers' blockers array
						for (int i = 0; i < s_blockers.Count; ++i)
						{
							if (i != attackerIndex)
							{
								s_blockers[i].Remove(blocker);
							}
						}
					}
				}

				Console.Write("<Blockers> ");
				for (int i = 0; i < s_blockers.Count; ++i)
				{
					if (s_blockers[i].Count == 0)
					{
						continue;
					}

                    var attackers = (Program.ActiveInteraction as Interactions.BlockPhase).DeclaredAttackers;

					Console.Write(String.Join(",", (from c in s_blockers[i] select c.Print()).ToArray())
                                  + " -> " + attackers[i].Print() + " ");
				}
				Console.WriteLine();
			}
		}
	}
}
