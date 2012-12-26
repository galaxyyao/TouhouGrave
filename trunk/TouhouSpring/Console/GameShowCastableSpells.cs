using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
    class GameShowCastableSpells : ICommandlet
    {
        public string Tag
        {
            get { return "game.castfrom"; }
        }

        public void Execute(params string[] args)
        {
            if (!(Program.ActiveInteraction is Interactions.TacticalPhase))
            {
                Console.WriteLine("ERROR: This command can only be invoked when in Tactical phase.");
                Console.WriteLine("");
                return;
            }

            var io = Program.ActiveInteraction as Interactions.TacticalPhase;
            var cards = io.CastFromSet;

            int counter = 0;
            for (int i = 0; i < cards.Count; ++i)
            {
                var card = cards[i];
                foreach (var spell in card.Spells)
                {
                    Console.WriteLine("> [{0}] {1} on {2} ({3})", ++counter, spell.Model.Name, card.Model.Name, GetLocation(card));
                }
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
