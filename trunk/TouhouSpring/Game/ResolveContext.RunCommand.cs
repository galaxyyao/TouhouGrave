using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
    partial class ResolveContext
    {
        // running command and the commands queued in Prolog phase form a "Timing Group";
        // commands in the "Timing Group" will effectively run at the same time by interleaving
        // their execution flow
        private Commands.BaseCommand m_timingGroupHead;
        private Commands.BaseCommand m_timingGroupTail;

        internal bool IsCommandRunnable(Commands.BaseCommand command)
        {
            Debug.Assert(RunningCommand == null);
            InitializeCommand(command);
            return !RunPrerequisite(command, false).Canceled;
        }

        private void RunMainAndEpilog(Commands.BaseCommand command)
        {
            Debug.Assert(command.Context == this);
            Debug.Assert(RunningCommand == null);
            RunningCommand = command;
            command.ExecutionPhase = Commands.CommandPhase.Main;
            GetCommandRunner(command.GetType()).RunMainAndEpilog(command);
            Game.Controller.OnCommandEnd(command);
            RunningCommand = null;
        }

        private void RunFull(Commands.BaseCommand command)
        {
            Debug.Assert(command.Context == this);
            Debug.Assert(RunningCommand == null);

            //////////////////////////////////////////////
            // Prolog

            m_timingGroupHead = m_timingGroupTail = command;

            for (var cmd = command; cmd != m_timingGroupTail.Next; cmd = cmd.Next)
            {
                RunningCommand = cmd;
                cmd.ExecutionPhase = Commands.CommandPhase.Prolog;
                // commands queued here will be put into {m_timingGroupHead, m_timingGroupTail} queue
                GetCommandRunner(cmd.GetType()).RunProlog(cmd);
                RunningCommand = null;
            }

            //////////////////////////////////////////////
            // Preemptive
            //
            // Keep stacking until no more stack is requested from Preemptive triggers

            while (true)
            {
                ResolveContext newStack = null;

                for (var cmd = m_timingGroupHead; cmd != m_timingGroupTail.Next; cmd = cmd.Next)
                {
                    RunningCommand = cmd;
                    cmd.ExecutionPhase = Commands.CommandPhase.Preemptive;
                    newStack = GetCommandRunner(cmd.GetType()).RunPreemptive(cmd);
                    RunningCommand = null;

                    if (newStack != null)
                    {
                        break;
                    }
                }

                if (newStack != null)
                {
                    Game.FlushResolveContext();
                    if (Abort)
                    {
                        return;
                    }
                }
                else
                {
                    break;
                }
            }

            //////////////////////////////////////////////
            // Main+Epilog

            for (var cmd = m_timingGroupHead; cmd != m_timingGroupTail.Next; cmd = cmd.Next)
            {
                RunMainAndEpilog(cmd);
            }

            m_timingGroupHead = m_timingGroupTail = null;
        }

        private CommandResult RunPrerequisite(Commands.BaseCommand command, bool resolveConditions)
        {
            Debug.Assert(command.Context == this);
            Debug.Assert(RunningCommand == null);
            RunningCommand = command;
            command.ExecutionPhase = Commands.CommandPhase.Prerequisite;

            var runner = GetCommandRunner(command.GetType());
            var ret = runner.RunPrerequisite(command);
            if (!ret.Canceled)
            {
                command.ExecutionPhase = Commands.CommandPhase.Condition;
                ret = ResolveConditions(!resolveConditions);
            }

            RunningCommand = null;

            return ret;
        }

        private ICommandRunner GetCommandRunner(Type commandType)
        {
            ICommandRunner runner;
            if (!s_commandRunnerCache.TryGetValue(commandType, out runner))
            {
                lock (s_commandRunnerCache)
                {
                    if (!s_commandRunnerCache.TryGetValue(commandType, out runner))
                    {
                        var runnerType = typeof(CommandRunner<>).MakeGenericType(commandType);
                        runner = runnerType.Assembly.CreateInstance(runnerType.FullName) as ICommandRunner;
                        s_commandRunnerCache.Add(commandType, runner);
                    }
                }
            }

            return runner;
        }
    }
}
