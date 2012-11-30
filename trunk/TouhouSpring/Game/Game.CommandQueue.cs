using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
    public partial class Game
    {
        private Queue<Commands.CommandContext> m_pendingCommands = new Queue<Commands.CommandContext>();

        public Commands.CommandContext RunningCommand
        {
            get; private set;
        }

        public void IssueCommands(params Commands.ICommand[] commands)
        {
            commands.ForEach(cmd => IssueCommand(cmd));
        }

        internal void IssueCommandsAndFlush(params Commands.ICommand[] commands)
        {
            IssueCommands(commands);
            FlushCommandQueue();
        }

        internal void FlushCommandQueue()
        {
            Debug.Assert(RunningCommand == null);

            while (m_pendingCommands.Count != 0)
            {
                RunningCommand = m_pendingCommands.Dequeue();
                RunningCommand.Run();
                RunningCommand = null;
            }
        }

        private void IssueCommand(Commands.ICommand command)
        {
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }

            command.Validate(this);

            // check whether a new command can be issued at this timing
            if (RunningCommand != null)
            {
                Debug.Assert(RunningCommand.Phase != Commands.ExecutionPhase.Pending);

                if (RunningCommand.Phase == Commands.ExecutionPhase.Prerequisite
                    || RunningCommand.Phase == Commands.ExecutionPhase.Setup)
                {
                    throw new InvalidOperationException(String.Format("Command can't be issued in {0} phase.", RunningCommand.Phase.ToString()));
                }
            }

            // enqueue a new context and chain with the previous one
            m_pendingCommands.Enqueue(new Commands.CommandContext(this, command, RunningCommand));
        }

        public void Resolve()
        {
            // TODO: (command) Resolve
        }
    }
}
