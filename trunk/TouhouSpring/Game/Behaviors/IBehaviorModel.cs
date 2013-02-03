using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public interface IBehaviorModel
    {
        string Name { get; set; }
        string ModelTypeName { get; }
        IBehavior CreateInitialized();
    }

    internal interface IInternalBehaviorModel : IBehaviorModel
    {
        IBehavior Instantiate();
        IBehavior CreateInitializedPersistent();
    }
}
