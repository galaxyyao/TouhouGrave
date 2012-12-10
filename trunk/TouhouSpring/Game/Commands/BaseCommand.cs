using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Commands
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

    public abstract partial class BaseCommand
    {
        public CommandPhase ExecutionPhase
        {
            get; internal set;
        }

        public Game Game
        {
            get; internal set;
        }

        public BaseCommand Previous
        {
            get; internal set;
        }

        internal abstract void ValidateOnIssue();
        internal abstract void ValidateOnRun();
        internal abstract void RunMain();

        protected void CheckPatchable(string propertyName)
        {
            if (ExecutionPhase != CommandPhase.Prolog)
            {
                throw new InvalidOperationException(String.Format("{0} can't be set when the command is read-only.", propertyName));
            }
        }
    }
}
