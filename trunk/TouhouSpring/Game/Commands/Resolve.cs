using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Commands
{
    public class Resolve : BaseCommand
    {
        internal override void ValidateOnIssue()
        {
        }

        internal override bool ValidateOnRun()
        {
            return true;
        }

        internal override void RunMain()
        {
            foreach (var card in Context.Game.Players
                .SelectMany(player => player.CardsOnBattlefield)
                .Where(card => card.Behaviors.Has<Behaviors.Warrior>()))
            {
                var warrior = card.Behaviors.Get<Behaviors.Warrior>();
                if (warrior.Life <= 0)
                {
                    Context.QueueCommand(new Commands.Kill(card, null));
                }
                else if (warrior.Life > warrior.MaxLife)
                {
                    warrior.Life = warrior.MaxLife;
                }
            }
        }
    }
}
