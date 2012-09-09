﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Commands
{
    public interface IPrologTrigger<TCommand> where TCommand : ICommand
    {
        void Run(CommandContext context);
    }
}