using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TouhouSpring.Commands
{
    public enum ExecutionPhase
    {
        Pending,
        Prerequisite,
        Setup,
        Prolog,
        Main,
        Epilog
    }

    public struct Result
    {
        public bool Canceled;
        public string Reason;

        public readonly static Result Pass = new Result { Canceled = false, Reason = null };
        
        public static Result Cancel(string reason = null)
        {
            return new Result { Canceled = true, Reason = reason };
        }
    }

    public class CommandContext
    {
        public ICommand Command
        {
            get; private set;
        }

        public ExecutionPhase Phase
        {
            get; private set;
        }

        public CommandContext Previous
        {
            get; private set;
        }

        public Game Game
        {
            get; private set;
        }

        public Result Result
        {
            get; private set;
        }

        private List<BaseCard> m_associates = new List<BaseCard>();

        internal CommandContext(Game game, ICommand command, CommandContext previous)
        {
            Command = command;
            Phase = ExecutionPhase.Pending;
            Previous = previous;
            Game = game;
        }

        internal void Run()
        {
            var commandType = Command.GetType();
            var method = typeof(CommandContext).GetMethod("RunTyped", BindingFlags.Instance | BindingFlags.NonPublic);
            method.MakeGenericMethod(commandType).Invoke(this, null);
        }

        private void RunTyped<TCommand>() where TCommand : Commands.ICommand
        {
            Debug.Assert(typeof(TCommand) == Command.GetType());

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
