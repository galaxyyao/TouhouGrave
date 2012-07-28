using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Commands
{
    public interface IPrerequisiteTrigger<TCommand> where TCommand : ICommand
    {
        bool Run(CommandContext context);
    }
}
