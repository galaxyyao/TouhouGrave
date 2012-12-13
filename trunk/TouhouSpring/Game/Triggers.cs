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

    public interface IPrerequisiteTrigger<TCommand> where TCommand : Commands.BaseCommand
    {
        CommandResult Run(TCommand command);
    }

    public interface ISetupTrigger<TCommand> where TCommand : Commands.BaseCommand
    {
        CommandResult Run(TCommand command);
    }

    public interface IPrologTrigger<TCommand> where TCommand : Commands.BaseCommand
    {
        void Run(TCommand command);
    }

    public interface IEpilogTrigger<TCommand> where TCommand : Commands.BaseCommand
    {
        void Run(TCommand command);
    }
}
