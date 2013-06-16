using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
    public partial class BaseController
    {
        internal void OnCommandEnd(Commands.BaseCommand command)
        {
            if (command is Commands.SummonMove<Commands.Sacrifice>)
            {
                var card = (command as Commands.SummonMove<Commands.Sacrifice>).Subject;
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
            else if (command is Commands.DrawMove<Commands.Hand>)
            {
                var card = (command as Commands.DrawMove<Commands.Hand>).Subject;
                new Interactions.NotifyCardEvent(Game, "OnCardDrawn", card).Run();
            }
            else if (command is Commands.EndTurn)
            {
                new Interactions.NotifyPlayerEvent(Game, "OnTurnEnded", (command as Commands.EndTurn).Player).Run();
            }
            // TODO: Commands.Kill
            else if (command is Commands.KillMove<Commands.Battlefield>)
            {
                var card = (command as Commands.KillMove<Commands.Battlefield>).Subject;
                new Interactions.NotifyCardEvent(Game, "OnCardDestroyed", card).Run();
            }
            else if (command is Commands.StartTurn)
            {
                new Interactions.NotifyPlayerEvent(Game, "OnTurnStarted", (command as Commands.StartTurn).Player).Run();
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
            else if (command is Commands.MoveCard<Commands.Hand, Commands.Battlefield>)
            {
                var card = (command as Commands.MoveCard<Commands.Hand, Commands.Battlefield>).Subject;
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
