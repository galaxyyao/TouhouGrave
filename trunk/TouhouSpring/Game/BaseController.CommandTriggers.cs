using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
    public partial class BaseController
    {
        internal void OnCommandBegin(Commands.BaseCommand command)
        {
        }

        internal void OnCommandEnd(Commands.BaseCommand command)
        {
            if (command is Commands.DrawCard)
            {
                var card = (command as Commands.DrawCard).CardDrawn;
                if (card.Owner == Player)
                {
                    new Interactions.NotifyCardEvent(this, "OnCardDrawn", card).Run();
                }
            }
            else if (command is Commands.PlayCard)
            {
                var card = (command as Commands.PlayCard).CardToPlay;
                if (card.Owner == Player)
                {
                    new Interactions.NotifyCardEvent(this, "OnCardPlayed", card).Run();
                }
            }
            else if (command is Commands.Summon)
            {
                var card = (command as Commands.Summon).CardSummoned;
                if (card.Owner == Player)
                {
                    new Interactions.NotifyCardEvent(this, "OnCardSummoned", card).Run();
                }
            }
            else if (command is Commands.DealDamageToPlayer)
            {
                var cmd = command as Commands.DealDamageToPlayer;
                if (cmd.Player == Player)
                {
                    new Interactions.NotifyControllerEvent(this, "OnPlayerDamaged", Player, String.Format(CultureInfo.CurrentCulture, "Damage:{0}", cmd.DamageToDeal)).Run();
                }
            }
            else if (command is Commands.Kill)
            {
                var card = (command as Commands.Kill).Target;
                if (card.Owner == Player)
                {
                    new Interactions.NotifyCardEvent(this, "OnCardDestroyed", card).Run();
                }
            }
            else if (command is Commands.CastSpell)
            {
                var spell = (command as Commands.CastSpell).Spell;
                if (spell.Host.Owner == Player)
                {
                    new Interactions.NotifySpellEvent(this, "OnSpellCasted", spell as Behaviors.ICastableSpell).Run();
                }
            }
        }

        internal void OnCommandCanceled(Commands.BaseCommand command, string reason)
        {
            if (command is Commands.PlayCard)
            {
                var card = (command as Commands.PlayCard).CardToPlay;
                if (card.Owner == Player)
                {
                    new Interactions.NotifyCardEvent((this), "OnCardPlayCanceled", card, reason).Run();
                }
            }
            else if (command is Commands.CastSpell)
            {
                var spell = (command as Commands.CastSpell).Spell;
                if (spell.Host.Owner == Player)
                {
                    new Interactions.NotifySpellEvent(this, "OnSpellCastCanceled", spell as Behaviors.ICastableSpell, reason).Run();
                }
            }
        }
    }
}
