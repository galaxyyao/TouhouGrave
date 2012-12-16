using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
    class GameShowSelectableCards : ICommandlet
    {
        public string Tag
        {
            get { return "game.selectfrom"; }
        }

        public void Execute(params string[] args)
        {
            if (!(Program.ActiveInteraction is Interactions.SelectCards))
            {
                Console.WriteLine("ERROR: This command can only be invoked when a SelectCards message is requested.");
                Console.WriteLine("");
                return;
            }

            var io = Program.ActiveInteraction as Interactions.SelectCards;

            for (int i = 0; i < io.SelectFromSet.Count; ++i)
            {
                Console.WriteLine("> [{0}] {1} ({2})", i + 1, io.SelectFromSet[i].Model.Name, GetLocation(io.SelectFromSet[i]));
            }
        }

        private string GetLocation(BaseCard card)
        {
            foreach (var player in Program.TouhouSpringGame.Players)
            {
                if (player.CardsOnHand.Contains(card))
                {
                    return player.Name + "'s hand";
                }
                else if (player.CardsOnBattlefield.Contains(card))
                {
                    return player.Name + "'s battlefield";
                }
            }

            return "#unknown location#";
        }
    }
}
