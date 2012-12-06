using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Commands
{
    public class ResetAccumulatedDamage : ICommand
    {
        public string Token
        {
            get { return "ResetAccuDmg"; }
        }

        public void Validate(Game game)
        {
        }

        public void RunMain(Game game)
        {
            foreach (var player in game.Players)
            {
                player.CardsOnBattlefield.Where(card => card.Behaviors.Has<Behaviors.Warrior>())
                    .ForEach(card => card.Behaviors.Get<Behaviors.Warrior>().AccumulatedDamage = 0);
            }
        }
    }
}
