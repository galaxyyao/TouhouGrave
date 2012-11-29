﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Commands
{
    public interface ISetupTrigger<TCommand> where TCommand : ICommand
    {
        Result Run(CommandContext context);
    }
}
