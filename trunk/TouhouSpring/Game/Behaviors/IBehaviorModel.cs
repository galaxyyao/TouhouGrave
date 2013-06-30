using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public interface IBehaviorModel
    {
        string Name { get; set; }
        string BehaviorTypeName { get; }
        bool IsBehaviorStatic { get; }
        IBehavior CreateInitialized();
    }

    internal interface IInternalBehaviorModel : IBehaviorModel
    {
        IBehavior Instantiate();
        IBehavior CreateInitializedPersistent();
    }
}
