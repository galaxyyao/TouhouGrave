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
        private Commands.BaseCommand m_commandQueueHead;
        private Commands.BaseCommand m_commandQueueTail;

        // a dictionary of cached command runners to avoid using reflection too intensively
        private static Dictionary<Type, ICommandRunner> s_commandRunnerMap = new Dictionary<Type, ICommandRunner>();

        public Commands.BaseCommand RunningCommand
        {
            get; private set;
        }

        public void QueueCommands(params Commands.BaseCommand[] commands)
        {
            commands.ForEach(cmd =>
            {
                if (cmd is Commands.Resolve)
                {
                    throw new ArgumentException("Resolve command can't be issued in behaviors.");
                }
                QueueCommand(cmd);
            });
        }

        public void QueueCommandsAndFlush(params Commands.BaseCommand[] commands)
        {
            QueueCommands(commands);
            FlushCommandQueue();
        }

        internal void FlushCommandQueue()
        {
            Debug.Assert(RunningCommand == null);

            bool commandFlushed = false;
            while (m_commandQueueHead != null)
            {
                commandFlushed = true;
                while (m_commandQueueHead != null)
                {
                    RunningCommand = m_commandQueueHead;
                    m_commandQueueHead = m_commandQueueHead.Next;
                    if (m_commandQueueHead == null)
                    {
                        Debug.Assert(RunningCommand == m_commandQueueTail);
                        m_commandQueueTail = null;
                    }
                    RunCommandGeneric(RunningCommand);
                    RunningCommand = null;
                }

                QueueCommand(new Commands.Resolve());
                RunningCommand = m_commandQueueHead;
                m_commandQueueHead = m_commandQueueHead.Next;
                if (m_commandQueueHead == null)
                {
                    Debug.Assert(RunningCommand == m_commandQueueTail);
                    m_commandQueueTail = null;
                }
                RunCommandGeneric(RunningCommand);
                RunningCommand = null;
            }

            if (commandFlushed)
            {
                Controller.OnCommandFlushed();
            }
        }

        internal bool IsCardPlayable(CardInstance card)
        {
            return IsCommandRunnable(new Commands.PlayCard(card));
        }

        internal bool IsCardActivatable(CardInstance card)
        {
            return IsCommandRunnable(new Commands.ActivateAssist(card));
        }

        internal bool IsCardRedeemable(CardInstance card)
        {
            return IsCommandRunnable(new Commands.Redeem(card));
        }

        internal bool IsSpellCastable(Behaviors.ICastableSpell spell)
        {
            return IsCommandRunnable(new Commands.CastSpell(spell));
        }

        private void QueueCommand(Commands.BaseCommand command)
        {
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }

            // check whether a new command can be queued at this timing
            if (RunningCommand != null)
            {
                Debug.Assert(RunningCommand.ExecutionPhase != Commands.CommandPhase.Pending);

                if (RunningCommand.ExecutionPhase == Commands.CommandPhase.Prerequisite)
                {
                    throw new InvalidOperationException("Command can't be queued in Prerequisite phase.");
                }
            }

            InitializeCommand(command);
            command.ValidateOnIssue();

            if (m_commandQueueTail == null)
            {
                Debug.Assert(m_commandQueueHead == null);
                m_commandQueueHead = m_commandQueueTail = command;
            }
            else
            {
                Debug.Assert(m_commandQueueTail.Next == null);
                m_commandQueueTail.Next = command;
                command.Prev = m_commandQueueTail;
                m_commandQueueTail = command;
            }
        }

        private void InitializeCommand(Commands.BaseCommand command)
        {
            command.ExecutionPhase = Commands.CommandPhase.Pending;
            command.Game = this;
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
            ICommandRunner runner;
            if (!s_commandRunnerMap.TryGetValue(command.GetType(), out runner))
            {
                lock (s_commandRunnerMap)
                {
                    if (!s_commandRunnerMap.TryGetValue(command.GetType(), out runner))
                    {
                        var runnerType = typeof(CommandRunner<>).MakeGenericType(command.GetType());
                        runner = runnerType.Assembly.CreateInstance(runnerType.FullName) as ICommandRunner;
                        s_commandRunnerMap.Add(command.GetType(), runner);
                    }
                }
            }

            runner.Run(command);
        }

        private CommandResult RunPrerequisiteGeneric(Commands.BaseCommand command)
        {
            ICommandRunner runner;
            if (!s_commandRunnerMap.TryGetValue(command.GetType(), out runner))
            {
                lock (s_commandRunnerMap)
                {
                    if (!s_commandRunnerMap.TryGetValue(command.GetType(), out runner))
                    {
                        var runnerType = typeof(CommandRunner<>).MakeGenericType(command.GetType());
                        runner = runnerType.Assembly.CreateInstance(runnerType.FullName) as ICommandRunner;
                        s_commandRunnerMap.Add(command.GetType(), runner);
                    }
                }
            }

            return runner.RunPrerequisite(command);
        }
    }
}
