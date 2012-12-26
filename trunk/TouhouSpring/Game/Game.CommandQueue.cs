using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TouhouSpring
{
    public partial class Game
    {
        private Queue<Commands.BaseCommand> m_pendingCommands = new Queue<Commands.BaseCommand>();

        public Commands.BaseCommand RunningCommand
        {
            get; private set;
        }

        public void IssueCommands(params Commands.BaseCommand[] commands)
        {
            commands.ForEach(cmd => IssueCommand(cmd));
        }

        internal void IssueCommandsAndFlush(params Commands.BaseCommand[] commands)
        {
            IssueCommands(commands);
            FlushCommandQueue();
        }

        internal void FlushCommandQueue()
        {
            Debug.Assert(RunningCommand == null);

            while (m_pendingCommands.Count != 0)
            {
                var cmd = m_pendingCommands.Dequeue();
                RunningCommand = cmd;
                RunCommandGeneric(cmd);
                RunningCommand = null;
            }

            IssueCommand(new Commands.Resolve());
            while (m_pendingCommands.Count != 0)
            {
                var cmd = m_pendingCommands.Dequeue();
                RunningCommand = cmd;
                RunCommandGeneric(cmd);
                RunningCommand = null;
            }
        }

        internal bool IsCardPlayable(BaseCard card)
        {
            Debug.Assert(RunningCommand == null);
            var cmd = new Commands.PlayCard(card);
            InitializeCommand(cmd);
            RunningCommand = cmd;
            bool ret = !RunPrerequisiteGeneric(cmd).Canceled;
            RunningCommand = null;
            return ret;
        }

        private void IssueCommand(Commands.BaseCommand command)
        {
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }

            InitializeCommand(command);
            command.ValidateOnIssue();

            // check whether a new command can be issued at this timing
            if (RunningCommand != null)
            {
                Debug.Assert(RunningCommand.ExecutionPhase != Commands.CommandPhase.Pending);

                if (RunningCommand.ExecutionPhase == Commands.CommandPhase.Prerequisite
                    || RunningCommand.ExecutionPhase == Commands.CommandPhase.Setup)
                {
                    throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture, "Command can't be issued in {0} phase.", RunningCommand.ExecutionPhase.ToString()));
                }
            }

            m_pendingCommands.Enqueue(command);
        }

        private void InitializeCommand(Commands.BaseCommand command)
        {
            command.ExecutionPhase = Commands.CommandPhase.Pending;
            command.Game = this;
            command.Previous = RunningCommand;
        }

        private void RunCommandGeneric(Commands.BaseCommand command)
        {
            var method = GetType().GetMethod("RunCommand", BindingFlags.Instance | BindingFlags.NonPublic).MakeGenericMethod(command.GetType());
            method.Invoke(this, new object[] { command });
        }

        private CommandResult RunPrerequisiteGeneric(Commands.BaseCommand command)
        {
            var method = GetType().GetMethod("RunPrerequisite", BindingFlags.Instance | BindingFlags.NonPublic).MakeGenericMethod(command.GetType());
            return (CommandResult)method.Invoke(this, new object[] { command });
        }

        private void RunCommand<TCommand>(TCommand command) where TCommand : Commands.BaseCommand
        {
            ////////////////////////////////////////////

            command.ExecutionPhase = Commands.CommandPhase.Prerequisite;
            foreach (var trigger in EnumerateCommandTargets(command).SelectMany(card =>
                                       card.Behaviors.OfType<IPrerequisiteTrigger<TCommand>>()
                                       ).ToList())
            {
                var result = trigger.Run(command);
                if (result.Canceled)
                {
                    Players.ForEach(player => player.Controller.OnCommandCanceled(command, result.Reason));
                    ClearReservations();
                    return;
                }
            }

            ////////////////////////////////////////////

            command.ExecutionPhase = Commands.CommandPhase.Setup;
            foreach (var trigger in EnumerateCommandTargets(command).SelectMany(card =>
                                       card.Behaviors.OfType<ISetupTrigger<TCommand>>()
                                       ).ToList())
            {
                var result = trigger.Run(command);
                if (result.Canceled)
                {
                    Players.ForEach(player => player.Controller.OnCommandCanceled(command, result.Reason));
                    ClearReservations();
                    return;
                }
            }

            ////////////////////////////////////////////

            ClearReservations();

            command.ExecutionPhase = Commands.CommandPhase.Prolog;
            Players.ForEach(player => player.Controller.OnCommandBegin(command));
            EnumerateCommandTargets(command).SelectMany(card => card.Behaviors.OfType<IPrologTrigger<TCommand>>())
                .ToList().ForEach(trigger => trigger.Run(command));

            ////////////////////////////////////////////

            command.ExecutionPhase = Commands.CommandPhase.Main;
            command.RunMain();

            ////////////////////////////////////////////

            command.ExecutionPhase = Commands.CommandPhase.Epilog;
            EnumerateCommandTargets(command).SelectMany(card => card.Behaviors.OfType<IEpilogTrigger<TCommand>>())
                .ToList().ForEach(trigger => trigger.Run(command));
            Players.ForEach(player => player.Controller.OnCommandEnd(command));
        }

        private CommandResult RunPrerequisite<TCommand>(TCommand command) where TCommand : Commands.BaseCommand
        {
            command.ExecutionPhase = Commands.CommandPhase.Prerequisite;
            foreach (var trigger in EnumerateCommandTargets(command)
                .SelectMany(card => card.Behaviors.OfType<IPrerequisiteTrigger<TCommand>>()).ToList())
            {
                var result = trigger.Run(command);
                if (result.Canceled)
                {
                    ClearReservations();
                    return result;
                }
            }

            ClearReservations();
            return CommandResult.Pass;
        }

        private IEnumerable<BaseCard> EnumerateCommandTargets(Commands.BaseCommand command)
        {
            if (command is Commands.PlayCard && command.ExecutionPhase < Commands.CommandPhase.Epilog)
            {
                var cardToPlay = (command as Commands.PlayCard).CardToPlay;
                if (cardToPlay != null)
                {
                    yield return cardToPlay;
                }
            }
            else if (command is Commands.Kill && command.ExecutionPhase == Commands.CommandPhase.Epilog)
            {
                var cardKilled = (command as Commands.Kill).Target;
                if (cardKilled != null)
                {
                    yield return cardKilled;
                }
            }

            foreach (var player in Players)
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
