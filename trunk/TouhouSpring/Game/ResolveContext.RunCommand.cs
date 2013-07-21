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
            if (command.ValidateOnRun() && command.DefaultValidateOnRun(this))
            {
                command.ExecutionPhase = Commands.CommandPhase.Main;
                command.RunMain();
                if (!(command is Commands.ISilentCommand))
                {
                    command.ExecutionPhase = Commands.CommandPhase.Epilog;
                    GetCommandRunner(command.GetType()).RunEpilog(command);
                }
            }
            Game.Controller.OnCommandEnd(command);
            RunningCommand = null;
        }

        private void RunFull(Commands.BaseCommand command)
        {
            Debug.Assert(command.Context == this);
            Debug.Assert(RunningCommand == null);

            if (command is Commands.ISilentCommand)
            {
                // epilog will be skipped
                RunMainAndEpilog(command);
                return;
            }

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

            bool firstTime = true;
            while (true)
            {
                ResolveContext newStack = null;

                for (var cmd = m_timingGroupHead; cmd != m_timingGroupTail.Next; cmd = cmd.Next)
                {
                    RunningCommand = cmd;
                    cmd.ExecutionPhase = Commands.CommandPhase.Preemptive;
                    newStack = GetCommandRunner(cmd.GetType()).RunPreemptive(cmd, firstTime);

                    if (newStack != null)
                    {
                        Game.StackAndFlush(newStack);
                        if (Abort)
                        {
                            return;
                        }
                        firstTime = false;
                        break;
                    }

                    RunningCommand = null;
                }

                if (newStack == null)
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
