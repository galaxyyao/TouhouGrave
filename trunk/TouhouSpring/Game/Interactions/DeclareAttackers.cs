using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Interactions
{
    public class DeclareAttackers : SelectCards
    {
        public DeclareAttackers(Player player)
            : base(player,
                   player.CardsOnBattlefield.Where(card =>
                       card.Behaviors.Has<Behaviors.Warrior>()
                       && card.Behaviors.Get<Behaviors.Warrior>().State == Behaviors.WarriorState.StandingBy).ToArray().ToIndexable(),
                   SelectMode.Multiple,
                   "Select warriors in battlefield to make them attackers.")
        { }
    }
}
