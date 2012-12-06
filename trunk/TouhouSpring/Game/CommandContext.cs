using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TouhouSpring
{
    public class CommandContext<TCommand> : IRunnableCommandContext
        where TCommand : ICommand
    {
        ICommand ICommandContext.Command
        {
            get { return Command; }
        }

        public TCommand Command
        {
            get; private set;
        }

        public ICommandContext Previous
        {
            get; private set;
        }

        public CommandPhase Phase
        {
            get; private set;
        }

        public Game Game
        {
            get; private set;
        }

        public CommandResult Result
        {
            get; private set;
        }

        internal CommandContext(Game game, TCommand command, ICommandContext previous)
        {
            Command = command;
            Previous = previous;
            Phase = CommandPhase.Pending;
            Game = game;
            Result = CommandResult.Pass;
        }

        void IRunnableCommandContext.Run()
        {
            ////////////////////////////////////////////

            Phase = CommandPhase.Prerequisite;
            foreach (var trigger in EnumerateCards().SelectMany(card =>
                                       card.Behaviors.OfType<IPrerequisiteTrigger<TCommand>>()
                                       ).ToList())
            {
                Result = trigger.Run(this);
                if (Result.Canceled)
                {
                    Game.Controllers.ForEach(ctrl => ctrl.OnCommandEnd(this));
                    Game.ClearReservations();
                    return;
                }
            }

            ////////////////////////////////////////////

            Phase = CommandPhase.Setup;
            foreach (var trigger in EnumerateCards().SelectMany(card =>
                                       card.Behaviors.OfType<ISetupTrigger<TCommand>>()
                                       ).ToList())
            {
                Result = trigger.Run(this);
                if (Result.Canceled)
                {
                    Game.Controllers.ForEach(ctrl => ctrl.OnCommandEnd(this));
                    Game.ClearReservations();
                    return;
                }
            }

            ////////////////////////////////////////////

            Game.ClearReservations();

            Phase = CommandPhase.Prolog;
            Game.Controllers.ForEach(ctrl => ctrl.OnCommandBegin(this));
            EnumerateCards().SelectMany(card =>
                                       card.Behaviors.OfType<IPrologTrigger<TCommand>>())
                .ToList().ForEach(trigger => trigger.Run(this));
            Game.Resolve();

            ////////////////////////////////////////////

            Phase = CommandPhase.Main;
            Command.RunMain(Game);
            Game.Resolve();

            ////////////////////////////////////////////

            Phase = CommandPhase.Epilog;
            EnumerateCards().SelectMany(card =>
                                       card.Behaviors.OfType<IEpilogTrigger<TCommand>>())
                .ToList().ForEach(trigger => trigger.Run(this));
            Game.Controllers.ForEach(ctrl => ctrl.OnCommandEnd(this));
            Game.Resolve();
        }

        private IEnumerable<BaseCard> EnumerateCards()
        {
            if (Command is Commands.PlayCard && Phase < CommandPhase.Epilog)
            {
                var cardToPlay = (Command as Commands.PlayCard).CardToPlay;
                if (cardToPlay != null)
                {
                    yield return cardToPlay;
                }
            }
            else if (Command is Commands.Kill && Phase == CommandPhase.Epilog)
            {
                var cardKilled = (Command as Commands.Kill).Target;
                if (cardKilled != null)
                {
                    yield return cardKilled;
                }
            }

            foreach (var player in Game.Players)
            {
                foreach (var card in player.CardsOnBattlefield)
                {
                    yield return card;
                }
                if (!player.CardsOnBattlefield.Contains(player.Hero.Host))
                {
                    yield return player.Hero.Host;
                }
            }
        }
    }
}
