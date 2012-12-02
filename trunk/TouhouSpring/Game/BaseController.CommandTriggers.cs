using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TouhouSpring.Commands;

namespace TouhouSpring
{
    public partial class BaseController
    {
        public void OnCommandBegin(ICommandContext context)
        {
        }

        public void OnCommandEnd(ICommandContext context)
        {
            if (context.Command is DrawCard && !context.Result.Canceled)
            {
                var card = (context.Command as DrawCard).CardDrawn;
                if (card.Owner == Player)
                {
                    new Interactions.NotifyCardEvent(this, "OnCardDrawn", card).Run();
                }
            }
            else if (context.Command is PlayCard)
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
            else if (context.Command is DealDamageToPlayer && !context.Result.Canceled)
            {
                var cmd = context.Command as DealDamageToPlayer;
                if (cmd.Target == Player)
                {
                    new Interactions.NotifyControllerEvent(this, "OnPlayerDamaged", Player, string.Format("Damage:{0}", cmd.DamageToDeal)).Run();
                }
            }
        }
    }
}
