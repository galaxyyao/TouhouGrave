using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Simulation
{
    class Branch
    {
        public Choice[] ChoicePath;
        public Game Result;
    }

    interface ISandbox
    {
        void Start();

        int BranchCount { get; }
        IEnumerable<Branch> Branches { get; }
    }
}
