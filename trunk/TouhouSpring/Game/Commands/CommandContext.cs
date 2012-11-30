using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TouhouSpring.Commands
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

        public ExecutionPhase Phase
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
            Phase = ExecutionPhase.Pending;
            Game = game;
            Result = CommandResult.Pass;
        }

        void IRunnableCommandContext.Run()
        {
            ////////////////////////////////////////////

            Phase = ExecutionPhase.Prerequisite;
            foreach (var trigger in EnumerateCards().SelectMany(card =>
                                       card.Behaviors.OfType<IPrerequisiteTrigger<TCommand>>()
                                       ).ToList())
            {
                Result = trigger.Run(this);
                if (Result.Canceled)
                {
                    Game.Controllers.ForEach(ctrl => ctrl.OnCommandEnd(this));
                    return;
                }
            }

            ////////////////////////////////////////////

            Phase = ExecutionPhase.Setup;
            foreach (var trigger in EnumerateCards().SelectMany(card =>
                                       card.Behaviors.OfType<ISetupTrigger<TCommand>>()
                                       ).ToList())
            {
                Result = trigger.Run(this);
                if (Result.Canceled)
                {
                    Game.Controllers.ForEach(ctrl => ctrl.OnCommandEnd(this));
                    return;
                }
            }

            ////////////////////////////////////////////

            Phase = ExecutionPhase.Prolog;
            Game.Controllers.ForEach(ctrl => ctrl.OnCommandBegin(this));
            EnumerateCards().SelectMany(card =>
                                       card.Behaviors.OfType<IPrologTrigger<TCommand>>())
                .ToList().ForEach(trigger => trigger.Run(this));
            Game.Resolve();

            ////////////////////////////////////////////

            Phase = ExecutionPhase.Main;
            Command.RunMain(Game);
            Game.Resolve();

            ////////////////////////////////////////////

            Phase = ExecutionPhase.Epilog;
            EnumerateCards().SelectMany(card =>
                                       card.Behaviors.OfType<IEpilogTrigger<TCommand>>())
                .ToList().ForEach(trigger => trigger.Run(this));
            Game.Controllers.ForEach(ctrl => ctrl.OnCommandEnd(this));
            Game.Resolve();
        }

        private IEnumerable<BaseCard> EnumerateCards()
        {
            if (Command is PlayCard)
            {
                yield return (Command as PlayCard).CardToPlay;
            }
            foreach (var player in Game.Players)
            {
                foreach (var card in player.CardsOnBattlefield)
                {
                    yield return card;
                }
            }
        }
    }
}
