using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace TouhouSpring.Commands
{
    public enum CommandPhase
    {
        Pending,
        Prerequisite,
        Condition,
        Prolog,
        Main,
        Epilog
    }

    public interface ICause { }

    public interface IInitiativeCommand
    {
        Player Initiator { get; }
    }

    public abstract partial class BaseCommand
    {
        public CommandPhase ExecutionPhase
        {
            get; internal set;
        }

        public BaseCommand Previous
        {
            get; internal set;
        }

        public ICause Cause
        {
            get; private set;
        }

        internal Game Game
        {
            get; set;
        }

        internal abstract void ValidateOnIssue();
        internal abstract void ValidateOnRun();
        internal abstract void RunMain();

        protected BaseCommand() { }
        protected BaseCommand(ICause cause) { Cause = cause; }

        protected void CheckPatchable(string propertyName)
        {
            if (ExecutionPhase != CommandPhase.Prolog)
            {
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture, "{0} can't be set when the command is read-only.", propertyName));
            }
        }
    }
}
