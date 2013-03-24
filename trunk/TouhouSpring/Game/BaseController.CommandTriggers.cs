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
            if (command is Commands.AddCardToManaPool)
            {
                var card = (command as Commands.AddCardToManaPool).CardAdded;
                new Interactions.NotifyCardEvent(Game, "OnCardAddedToManaPool", card).Run();
            }
            else if (command is Commands.CastSpell)
            {
                var spell = (command as Commands.CastSpell).Spell;
                new Interactions.NotifySpellEvent(Game, "OnSpellCasted", spell as Behaviors.ICastableSpell).Run();
            }
            else if (command is Commands.SubtractPlayerLife)
            {
                var cmd = command as Commands.SubtractPlayerLife;
                new Interactions.NotifyPlayerEvent(Game, "OnPlayerLifeSubtracted", cmd.Player, String.Format(CultureInfo.CurrentCulture, "Damage:{0}", cmd.FinalAmount)).Run();
            }
            else if (command is Commands.DrawCard)
            {
                var card = (command as Commands.DrawCard).CardDrawn;
                new Interactions.NotifyCardEvent(Game, "OnCardDrawn", card).Run();
            }
            else if (command is Commands.EndTurn)
            {
                new Interactions.NotifyPlayerEvent(Game, "OnTurnEnded", (command as Commands.EndTurn).Player).Run();
            }
            else if (command is Commands.Kill)
            {
                var card = (command as Commands.Kill).Target;
                new Interactions.NotifyCardEvent(Game, "OnCardDestroyed", card).Run();
            }
            else if (command is Commands.PlayCard)
            {
                var card = (command as Commands.PlayCard).CardToPlay;
                new Interactions.NotifyCardEvent(Game, "OnCardPlayed", card).Run();
            }
            else if (command is Commands.StartTurn)
            {
                new Interactions.NotifyPlayerEvent(Game, "OnTurnStarted", (command as Commands.StartTurn).Player).Run();
            }
            else if (command is Commands.Summon)
            {
                var card = (command as Commands.Summon).CardSummoned;
                new Interactions.NotifyCardEvent(Game, "OnCardSummoned", card).Run();
            }

            if (command is Commands.IInitiativeCommand)
            {
                new Interactions.NotifyGameEvent(Game, "OnInitiativeCommandEnd", null).Run();
            }
        }

        internal void OnCommandCanceled(Commands.BaseCommand command, string reason)
        {
            if (command is Commands.CastSpell)
            {
                var spell = (command as Commands.CastSpell).Spell;
                new Interactions.NotifySpellEvent(Game, "OnSpellCastCanceled", spell as Behaviors.ICastableSpell, reason).Run();
            }
            else if (command is Commands.PlayCard)
            {
                var card = (command as Commands.PlayCard).CardToPlay;
                new Interactions.NotifyCardEvent(Game, "OnCardPlayCanceled", card, reason).Run();
            }

            if (command is Commands.IInitiativeCommand)
            {
                new Interactions.NotifyGameEvent(Game, "OnInitiativeCommandCanceled", null).Run();
            }
        }

        public virtual void OnRespondBack(Interactions.BaseInteraction io, object result) { }
        public virtual void OnCommandFlushed() { }
    }
}
