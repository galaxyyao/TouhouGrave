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

    public interface IGlobalPrerequisiteTrigger<in TCommand> where TCommand : Commands.ICommand
    {
        CommandResult RunGlobalPrerequisite(TCommand command);
    }

    public interface ILocalPrerequisiteTrigger<in TCommand> where TCommand : Commands.ICommand
    {
        CommandResult RunLocalPrerequisite(TCommand command);
    }

    public interface IGlobalPreemptiveTrigger<in TCommand> where TCommand : Commands.ICommand
    {
        ResolveContext RunGlobalPreemptive(TCommand command, bool firstTimeTriggering);
    }

    public interface ILocalPreemptiveTrigger<in TCommand> where TCommand : Commands.ICommand
    {
        ResolveContext RunLocalPreemptive(TCommand command, bool firstTimeTriggering);
    }

    public interface IGlobalPrologTrigger<in TCommand> where TCommand : Commands.ICommand
    {
        void RunGlobalProlog(TCommand command);
    }

    public interface ILocalPrologTrigger<in TCommand> where TCommand : Commands.ICommand
    {
        void RunLocalProlog(TCommand command);
    }

    public interface IGlobalEpilogTrigger<in TCommand> where TCommand : Commands.ICommand
    {
        void RunGlobalEpilog(TCommand command);
    }

    public interface ILocalEpilogTrigger<in TCommand> where TCommand : Commands.ICommand
    {
        void RunLocalEpilog(TCommand command);
    }
}
