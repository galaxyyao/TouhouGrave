using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
    internal partial class ResolveContext
    {
        private Commands.BaseCommand m_commandQueueHead;
        private Commands.BaseCommand m_commandQueueTail;

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

        internal bool IsCommandRunnable(Commands.BaseCommand command)
        {
            Debug.Assert(RunningCommand == null);
            InitializeCommand(command);
            RunningCommand = command;
            bool ret = !RunPrerequisiteGeneric(command).Canceled;
            RunningCommand = null;
            return ret;
        }

        internal void QueueCommand(Commands.BaseCommand command)
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
                m_commandQueueHead = command;
            }
            else
            {
                Debug.Assert(m_commandQueueTail.Next == null);
                m_commandQueueTail.Next = command;
            }
            m_commandQueueTail = command;
        }

        private void QueueCommandAtHead(Commands.BaseCommand command)
        {
            // can only queue command at head when issueing resource consuming commands
            Debug.Assert(RunningCommand != null && RunningCommand.ExecutionPhase == Commands.CommandPhase.Condition);
            InitializeCommand(command);
            command.ValidateOnIssue();

            command.Next = m_commandQueueHead;
            m_commandQueueHead = command;
            if (m_commandQueueTail == null)
            {
                m_commandQueueTail = m_commandQueueHead;
            }
        }

        private void InitializeCommand(Commands.BaseCommand command)
        {
            command.ExecutionPhase = Commands.CommandPhase.Pending;
            command.Context = this;
        }

        private Commands.BaseCommand DequeCommand()
        {
            Debug.Assert(m_commandQueueHead != null);

            var ret = m_commandQueueHead;
            m_commandQueueHead = m_commandQueueHead.Next;
            if (m_commandQueueHead == null)
            {
                Debug.Assert(ret == m_commandQueueTail);
                m_commandQueueTail = null;
            }

            return ret;
        }
    }
}
