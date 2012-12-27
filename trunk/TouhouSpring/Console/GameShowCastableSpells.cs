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
            var spells = io.CastFromSet;

            for (int i = 0; i < spells.Count; ++i)
            {
                Console.WriteLine("> [{0}] {1} on {2} ({3})", i, spells[i].Model.Name, spells[i].Host.Model.Name, GetLocation(spells[i].Host));
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
