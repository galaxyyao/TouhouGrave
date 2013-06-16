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

    public interface IInitiativeCommand
    {
        Player Initiator { get; }
    }

    public interface ICommand
    {
        CommandPhase ExecutionPhase { get; }
        ICause Cause { get; }
    }
}
