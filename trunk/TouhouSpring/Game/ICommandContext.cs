using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
    public enum CommandPhase
    {
        Pending,
        Prerequisite,
        Setup,
        Prolog,
        Main,
        Epilog
    }

    public struct CommandResult
    {
        public bool Canceled;
        public string Reason;

        public readonly static CommandResult Pass = new CommandResult { Canceled = false, Reason = null };

        public static CommandResult Cancel(string reason = null)
        {
            return new CommandResult { Canceled = true, Reason = reason };
        }
    }

    public interface ICommandContext
    {
        ICommand Command { get; }
        ICommandContext Previous { get; }
        CommandPhase Phase { get; }
        Game Game { get; }
        CommandResult Result { get; }
    }

    internal interface IRunnableCommandContext : ICommandContext
    {
        void Run();
    }
}
