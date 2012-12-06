using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
    public partial class BaseController
    {
        public void OnCommandBegin(ICommandContext context)
        {
        }

        public void OnCommandEnd(ICommandContext context)
        {
            if (context.Command is Commands.DrawCard && !context.Result.Canceled)
            {
                var card = (context.Command as Commands.DrawCard).CardDrawn;
                if (card.Owner == Player)
                {
                    new Interactions.NotifyCardEvent(this, "OnCardDrawn", card).Run();
                }
            }
            else if (context.Command is Commands.PlayCard)
            {
                var card = (context.Command as Commands.PlayCard).CardToPlay;
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
            else if (context.Command is Commands.DealDamageToPlayer && !context.Result.Canceled)
            {
                var cmd = context.Command as Commands.DealDamageToPlayer;
                if (cmd.Player == Player)
                {
                    new Interactions.NotifyControllerEvent(this, "OnPlayerDamaged", Player, string.Format("Damage:{0}", cmd.DamageToDeal)).Run();
                }
            }
            else if (context.Command is Commands.Kill && !context.Result.Canceled)
            {
                var card = (context.Command as Commands.Kill).Target;
                if (card.Owner == Player)
                {
                    new Interactions.NotifyCardEvent(this, "OnCardDestroyed", card).Run();
                }
            }
            else if (context.Command is Commands.CastSpell)
            {
                var spell = (context.Command as Commands.CastSpell).Spell;
                if (spell.Host.Owner == Player)
                {
                    if (!context.Result.Canceled)
                    {
                        new Interactions.NotifySpellEvent(this, "OnSpellCasted", spell).Run();
                    }
                    else
                    {
                        new Interactions.NotifySpellEvent(this, "OnSpellCastCanceled", spell, context.Result.Reason).Run();
                    }
                }
            }
        }
    }
}
