using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
    public partial class ResolveContext
    {
        private Commands.BaseCommand m_resourceCommandQueueHead;
        private Commands.BaseCommand m_resourceCommandQueueTail;
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

        internal void QueueCommandsAndFlush(params Commands.BaseCommand[] commands)
        {
            QueueCommands(commands);
            FlushCommandQueue();
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

            if (RunningCommand != null && RunningCommand.ExecutionPhase == Commands.CommandPhase.Prolog)
            {
                Debug.Assert(m_timingGroupHead != null && m_timingGroupTail != null);
                command.Next = m_timingGroupTail.Next;
                m_timingGroupTail.Next = command;
                m_timingGroupTail = command;
            }
            else
            {
                CommandQueue.PushTail(command, ref m_commandQueueHead, ref m_commandQueueTail);
            }
        }

        private void QueueResourceCommand(Commands.BaseCommand command)
        {
            // can only queue command at head when issueing resource consuming commands
            Debug.Assert(RunningCommand != null && RunningCommand.ExecutionPhase == Commands.CommandPhase.Condition);
            InitializeCommand(command);
            command.ValidateOnIssue();

            CommandQueue.PushTail(command, ref m_resourceCommandQueueHead, ref m_resourceCommandQueueTail);
        }

        private void InitializeCommand(Commands.BaseCommand command)
        {
            command.ExecutionPhase = Commands.CommandPhase.Pending;
            command.Context = this;
        }

        private Commands.BaseCommand DequeCommand()
        {
            return CommandQueue.PopHead(ref m_commandQueueHead, ref m_commandQueueTail);
        }

        private Commands.BaseCommand DequeResourceCommand()
        {
            return CommandQueue.PopHead(ref m_resourceCommandQueueHead, ref m_resourceCommandQueueTail);
        }
    }
}
