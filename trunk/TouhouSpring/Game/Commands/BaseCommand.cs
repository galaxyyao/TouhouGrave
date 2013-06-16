using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace TouhouSpring.Commands
{
    public abstract partial class BaseCommand : ICommand
    {
        public CommandPhase ExecutionPhase
        {
            get; internal set;
        }

        public ICause Cause
        {
            get; private set;
        }

        internal ResolveContext Context
        {
            get; set;
        }

        // for command list
        internal BaseCommand Next
        {
            get; set;
        }

        internal abstract void ValidateOnIssue();
        internal abstract bool ValidateOnRun();
        internal abstract void RunMain();

        protected BaseCommand() { }
        protected BaseCommand(ICause cause) { Cause = cause; }

        protected void CheckPatchable(string propertyName)
        {
            if (ExecutionPhase != CommandPhase.Prolog
                && ExecutionPhase != CommandPhase.Preemptive)
            {
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture, "{0} can't be set when the command is read-only.", propertyName));
            }
        }
    }
}
