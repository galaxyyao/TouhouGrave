using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TouhouSpring.Commands;

namespace TouhouSpring
{
    public partial class BaseController
    {
        public void OnCommandBegin(CommandContext context)
        {
        }

        public void OnCommandEnd(CommandContext context)
        {
            if (context.Command is Commands.DrawCard && !context.Result.Canceled)
            {
                var card = (context.Command as DrawCard).CardDrawn;
                if (card.Owner == Player)
                {
                    new Interactions.NotifyCardEvent(this, "OnCardDrawn", card).Run();
                }
            }
            else if (context.Command is Commands.PlayCard)
            {
                var card = (context.Command as PlayCard).CardToPlay;
                if (card.Owner == Player)
                {
                    if (!context.Result.Canceled)
                    {
                        new Interactions.NotifyCardEvent(this, "OnCardPlayed", card).Run();
                    }
                    else
                    {
                        new Interactions.NotifyCardEvent((this), "OnCardPlayCanceled", card, context.Result.Reason).Run();
                    }
                }
            }
        }
    }
}
