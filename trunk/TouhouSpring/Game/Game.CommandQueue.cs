﻿using System;
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
            commands.ForEach(cmd =>
            {
                if (cmd is Commands.Resolve)
                {
                    throw new ArgumentException("Resolve command can't be issued in behaviors.");
                }
                IssueCommand(cmd);
            });
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
                while (m_pendingCommands.Count != 0)
                {
                    var cmd = m_pendingCommands.Dequeue();
                    RunningCommand = cmd;
                    RunCommandGeneric(cmd);
                    RunningCommand = null;
                }

                IssueCommand(new Commands.Resolve());
                RunningCommand = m_pendingCommands.Dequeue();
                RunCommandGeneric(RunningCommand);
                RunningCommand = null;
            }
        }

        internal bool IsCardPlayable(BaseCard card)
        {
            return IsCommandRunnable(new Commands.PlayCard(card));
        }

        internal bool IsCardActivatable(BaseCard card)
        {
            return IsCommandRunnable(new Commands.ActivateAssist(card));
        }

        internal bool IsCardRedeemable(BaseCard card)
        {
            return IsCommandRunnable(new Commands.Redeem(card));
        }

        internal bool IsSpellCastable(Behaviors.ICastableSpell spell)
        {
            return IsCommandRunnable(new Commands.CastSpell(spell));
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

                if (RunningCommand.ExecutionPhase == Commands.CommandPhase.Prerequisite)
                {
                    throw new InvalidOperationException("Command can't be issued in Prerequisite phase.");
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

        private bool IsCommandRunnable(Commands.BaseCommand command)
        {
            Debug.Assert(RunningCommand == null);
            InitializeCommand(command);
            RunningCommand = command;
            bool ret = !RunPrerequisiteGeneric(command).Canceled;
            RunningCommand = null;
            return ret;
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
            command.ValidateOnRun();

            ////////////////////////////////////////////

            command.ExecutionPhase = Commands.CommandPhase.Prerequisite;
            foreach (var trigger in EnumerateCommandTargets(command).SelectMany(card =>
                                       card.Behaviors.OfType<IPrerequisiteTrigger<TCommand>>()
                                       ).ToList())
            {
                var result = trigger.RunPrerequisite(command);
                if (result.Canceled)
                {
                    Controller.OnCommandCanceled(command, result.Reason);
                    ClearConditions();
                    return;
                }
            }

            ////////////////////////////////////////////

            command.ExecutionPhase = Commands.CommandPhase.Condition;
            var conditionResult = ResolveConditions(false);
            if (conditionResult.Canceled)
            {
                Controller.OnCommandCanceled(command, conditionResult.Reason);
                ClearConditions();
                return;
            }

            ////////////////////////////////////////////

            command.ExecutionPhase = Commands.CommandPhase.Prolog;
            Controller.OnCommandBegin(command);
            EnumerateCommandTargets(command).SelectMany(card => card.Behaviors.OfType<IPrologTrigger<TCommand>>())
                .ToList().ForEach(trigger => trigger.RunProlog(command));

            ////////////////////////////////////////////

            command.ExecutionPhase = Commands.CommandPhase.Main;
            command.RunMain();

            ////////////////////////////////////////////

            command.ExecutionPhase = Commands.CommandPhase.Epilog;
            EnumerateCommandTargets(command).SelectMany(card => card.Behaviors.OfType<IEpilogTrigger<TCommand>>())
                .ToList().ForEach(trigger => trigger.RunEpilog(command));
            Controller.OnCommandEnd(command);

            ClearConditions();
        }

        private CommandResult RunPrerequisite<TCommand>(TCommand command) where TCommand : Commands.BaseCommand
        {
            command.ExecutionPhase = Commands.CommandPhase.Prerequisite;
            foreach (var trigger in EnumerateCommandTargets(command)
                .SelectMany(card => card.Behaviors.OfType<IPrerequisiteTrigger<TCommand>>()).ToList())
            {
                var result = trigger.RunPrerequisite(command);
                if (result.Canceled)
                {
                    ClearConditions();
                    return result;
                }
            }

            var ret = ResolveConditions(true);
            ClearConditions();
            return ret;
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
            else if (command is Commands.ActivateAssist && command.ExecutionPhase < Commands.CommandPhase.Epilog)
            {
                var cardToActivate = (command as Commands.ActivateAssist).CardToActivate;
                if (cardToActivate != null)
                {
                    yield return cardToActivate;
                }
            }
            else if (command is Commands.ActivateAssist && command.ExecutionPhase == Commands.CommandPhase.Epilog)
            {
                var previouslyActivated = (command as Commands.ActivateAssist).PreviouslyActivatedCard;
                if (previouslyActivated != null)
                {
                    yield return previouslyActivated;
                }
            }
            else if (command is Commands.Redeem)
            {
                var cardToRedeem = (command as Commands.Redeem).Target;
                if (cardToRedeem != null)
                {
                    yield return cardToRedeem;
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
                if (player.ActivatedAssist != null)
                {
                    yield return player.ActivatedAssist;
                }
            }
        }
    }
}
