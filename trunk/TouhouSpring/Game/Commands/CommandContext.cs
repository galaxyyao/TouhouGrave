using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TouhouSpring.Commands
{
    // TODO: (command) Add associates
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

        public CommandContext Parent
        {
            get; private set;
        }

        public Game Game
        {
            get; private set;
        }

        private List<BaseCard> m_associates = new List<BaseCard>();

        internal CommandContext(Game game, ICommand command, CommandContext parent)
        {
            Command = command;
            Phase = ExecutionPhase.Inactive;
            Parent = parent;
            Game = game;
        }

        internal void Run<TCommand>() where TCommand : Commands.ICommand
        {
            Debug.Assert(typeof(TCommand) == Command.GetType());

            ////////////////////////////////////////////

            Phase = ExecutionPhase.Prerequisite;
            foreach (var trigger in EnumerateCards().SelectMany(card =>
                                       card.Behaviors.OfType<IPrerequisiteTrigger<TCommand>>()
                                       ).ToList())
            {
                if (!trigger.Run(this))
                {
                    return;
                }
            }

            ////////////////////////////////////////////

            Phase = ExecutionPhase.Setup;
            foreach (var trigger in EnumerateCards().SelectMany(card =>
                                       card.Behaviors.OfType<ISetupTrigger<TCommand>>()
                                       ).ToList())
            {
                if (!trigger.Run(this))
                {
                    return;
                }
            }

            ////////////////////////////////////////////

            Phase = ExecutionPhase.Prolog;
            Game.Controllers.OfType<IPrologTrigger<TCommand>>().ForEach(trigger => trigger.Run(this));
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
            Game.Controllers.OfType<IEpilogTrigger<TCommand>>().ForEach(trigger => trigger.Run(this));
            EnumerateCards().SelectMany(card =>
                                       card.Behaviors.OfType<IEpilogTrigger<TCommand>>())
                .ToList().ForEach(trigger => trigger.Run(this));
            Game.Resolve();
        }

        private IEnumerable<BaseCard> EnumerateCards()
        {
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
