using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public interface IStatusEffect
    {
        string IconUri { get; }
        string Text { get; }
    }
}
