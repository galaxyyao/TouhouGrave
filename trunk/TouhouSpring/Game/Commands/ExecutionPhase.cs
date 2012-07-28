using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Commands
{
    public enum ExecutionPhase
    {
        Inactive,
        Prerequisite,
        Setup,
        Prolog,
        Main,
        Epilog
    }
}
