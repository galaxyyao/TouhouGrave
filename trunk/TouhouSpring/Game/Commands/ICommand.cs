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
        Condition,
        Preemptive,
        Prolog,
        Main,
        Epilog
    }

    public interface ICause { }

    public interface ICommand
    {
        CommandPhase ExecutionPhase { get; }
        ICause Cause { get; }
    }

    public interface IInitiativeCommand : ICommand
    {
        Player Initiator { get; }
    }

    // Silent command won't trigger any triggers
    public interface ISilentCommand : ICommand
    { }
}
