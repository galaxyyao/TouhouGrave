using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
    partial class ResolveContext
    {
        // a dictionary of cached command runners to avoid using reflection too intensively
        private static Dictionary<Type, Game.ICommandRunner> s_commandRunnerCache = new Dictionary<Type, Game.ICommandRunner>();

        internal void FlushCommandQueue()
        {
            Debug.Assert(RunningCommand == null);

            if (m_commandQueueHead is Commands.IInitiativeCommand)
            {
                // collect conditions: resources + targets
                RunningCommand = m_commandQueueHead;
                var runner = GetCommandRunner(RunningCommand.GetType());

                RunningCommand.ExecutionPhase = Commands.CommandPhase.Prerequisite;
                var result = runner.RunPrerequisite(RunningCommand);
                if (!result.Canceled)
                {
                    // commands consuming resources will be emitted at the head of the queue
                    RunningCommand.ExecutionPhase = Commands.CommandPhase.Condition;
                    result = ResolveConditions(false);
                }

                RunningCommand = null;

                if (result.Canceled)
                {
                    // all remaining commands are discarded
                    return;
                }
            }

            Debug.Assert(RunningCommand == null);

            bool commandFlushed = false;
            while (m_commandQueueHead != null)
            {
                commandFlushed = true;
                while (m_commandQueueHead != null)
                {
                    RunningCommand = DequeCommand();
                    RunCommandGeneric(RunningCommand);
                    RunningCommand = null;
                }

                QueueCommand(new Commands.Resolve());
                RunningCommand = DequeCommand();
                RunCommandGeneric(RunningCommand);
                RunningCommand = null;
            }

            if (commandFlushed)
            {
                Game.Controller.OnCommandFlushed();
            }
        }

        private void RunCommandGeneric(Commands.BaseCommand command)
        {
            Debug.Assert(command.Context == this);
            GetCommandRunner(command.GetType()).Run(command);
        }

        private CommandResult RunPrerequisiteGeneric(Commands.BaseCommand command)
        {
            Debug.Assert(command.Context == this);
            var runner = GetCommandRunner(command.GetType());
            command.ExecutionPhase = Commands.CommandPhase.Prerequisite;
            var ret = runner.RunPrerequisite(command);
            if (!ret.Canceled)
            {
                command.ExecutionPhase = Commands.CommandPhase.Condition;
                ret = ResolveConditions(true);
            }
            return ret;
        }

        private Game.ICommandRunner GetCommandRunner(Type commandType)
        {
            Game.ICommandRunner runner;
            if (!s_commandRunnerCache.TryGetValue(commandType, out runner))
            {
                lock (s_commandRunnerCache)
                {
                    if (!s_commandRunnerCache.TryGetValue(commandType, out runner))
                    {
                        var runnerType = typeof(Game.CommandRunner<>).MakeGenericType(commandType);
                        runner = runnerType.Assembly.CreateInstance(runnerType.FullName) as Game.ICommandRunner;
                        s_commandRunnerCache.Add(commandType, runner);
                    }
                }
            }

            return runner;
        }
    }
}
