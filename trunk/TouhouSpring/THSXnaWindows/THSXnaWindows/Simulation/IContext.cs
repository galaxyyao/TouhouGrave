using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Simulation
{
    interface IContext
    {
        int CurrentBranchDepth { get; }
        int CurrentBranchOrder { get; }
        Choice[] CurrentBranchChoicePath { get; }
    }
}
