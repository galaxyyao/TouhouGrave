using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Commands
{
    public class Resolve : BaseCommand, ICause
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
                .Where(card => card.Warrior != null))
            {
                if (card.Warrior.Life <= 0)
                {
                    Context.QueueCommand(new Commands.MoveCard(card, SystemZone.Graveyard, this));
                }
                else if (card.Warrior.Life > card.Warrior.MaxLife)
                {
                    card.Warrior.Life = card.Warrior.MaxLife;
                }
            }
        }
    }
}
