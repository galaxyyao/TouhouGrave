using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
    public partial class Game
    {
        public Commands.CommandContext RunningCommand
        {
            get; private set;
        }

        public void RunCommand<TCommand>(TCommand command)
            where TCommand : Commands.ICommand
        {
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }

            command.Validate(this);

            // check whether a new command can be issued at this timing
            if (RunningCommand != null)
            {
                Debug.Assert(RunningCommand.Phase != Commands.ExecutionPhase.Inactive);

                if (RunningCommand.Phase == Commands.ExecutionPhase.Prerequisite
                    || RunningCommand.Phase == Commands.ExecutionPhase.Setup)
                {
                    throw new InvalidOperationException(String.Format("Command can't be issued in {0} phase.", RunningCommand.Phase.ToString()));
                }
            }

            // create a new context and chain with the previous one through Parent member
            RunningCommand = new Commands.CommandContext(this, command, RunningCommand);

            // start running
            RunningCommand.Run<TCommand>();

            // restore the stack
            RunningCommand = RunningCommand.Parent;
        }

        public void Resolve()
        {
            // TODO: (command) Resolve
        }
    }
}
