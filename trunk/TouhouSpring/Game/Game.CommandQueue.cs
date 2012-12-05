using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TouhouSpring
{
    public partial class Game
    {
        private Queue<IRunnableCommandContext> m_pendingCommands = new Queue<IRunnableCommandContext>();

        public ICommandContext RunningCommand
        {
            get; private set;
        }

        public void IssueCommands(params ICommand[] commands)
        {
            commands.ForEach(cmd => IssueCommand(cmd));
        }

        internal void IssueCommandsAndFlush(params ICommand[] commands)
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
                cmd.Run();
                RunningCommand = null;
            }
        }

        private void IssueCommand(ICommand command)
        {
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }

            command.Validate(this);

            // check whether a new command can be issued at this timing
            if (RunningCommand != null)
            {
                Debug.Assert(RunningCommand.Phase != CommandPhase.Pending);

                if (RunningCommand.Phase == CommandPhase.Prerequisite
                    || RunningCommand.Phase == CommandPhase.Setup)
                {
                    throw new InvalidOperationException(String.Format("Command can't be issued in {0} phase.", RunningCommand.Phase.ToString()));
                }
            }

            // enqueue a new context and chain with the previous one
            var cmdCtxType = typeof(CommandContext<>).MakeGenericType(command.GetType());
            var cmdCtxCtor = cmdCtxType.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)[0];
            var cmdCtx = cmdCtxCtor.Invoke(new object[] { this, command, RunningCommand });
            m_pendingCommands.Enqueue(cmdCtx as IRunnableCommandContext);
        }

        public void Resolve()
        {
            // TODO: (command) Resolve
        }
    }
}
