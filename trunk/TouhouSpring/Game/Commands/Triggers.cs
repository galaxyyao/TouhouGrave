using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Commands
{
    public interface IPrerequisiteTrigger<TCommand> where TCommand : ICommand
    {
        CommandResult Run(CommandContext<TCommand> context);
    }

    public interface ISetupTrigger<TCommand> where TCommand : ICommand
    {
        CommandResult Run(CommandContext<TCommand> context);
    }

    public interface IPrologTrigger<TCommand> where TCommand : ICommand
    {
        void Run(CommandContext<TCommand> context);
    }

    public interface IEpilogTrigger<TCommand> where TCommand : ICommand
    {
        void Run(CommandContext<TCommand> context);
    }
}
