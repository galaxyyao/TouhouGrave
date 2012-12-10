using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Commands
{
    public class ResetAccumulatedDamage : BaseCommand
    {
        internal override void ValidateOnIssue()
        {
        }

        internal override void ValidateOnRun()
        {
        }

        internal override void RunMain()
        {
            foreach (var player in Game.Players)
            {
                player.CardsOnBattlefield.Where(card => card.Behaviors.Has<Behaviors.Warrior>())
                    .ForEach(card => card.Behaviors.Get<Behaviors.Warrior>().AccumulatedDamage = 0);
            }
        }
    }
}
