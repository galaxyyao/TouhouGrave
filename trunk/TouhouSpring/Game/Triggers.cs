using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
    public struct CommandResult
    {
        public bool Canceled;
        public string Reason;

        public readonly static CommandResult Pass = new CommandResult { Canceled = false, Reason = null };

        public static CommandResult Cancel()
        {
            return Cancel(String.Empty);
        }

        public static CommandResult Cancel(string reason)
        {
            return new CommandResult { Canceled = true, Reason = reason };
        }
    }

    public interface IPrerequisiteTrigger<in TCommand> where TCommand : Commands.BaseCommand
    {
        CommandResult RunPrerequisite(TCommand command);
    }

    public interface IPreemptiveTrigger<in TCommand> where TCommand : Commands.BaseCommand
    {
        ResolveContext RunPreemptive(TCommand command);
    }

    public interface IPrologTrigger<in TCommand> where TCommand : Commands.BaseCommand
    {
        void RunProlog(TCommand command);
    }

    public interface IEpilogTrigger<in TCommand> where TCommand : Commands.BaseCommand
    {
        void RunEpilog(TCommand command);
    }
}
