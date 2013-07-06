using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public interface IBehavior
    {
        CardInstance Host { get; }
        bool Persistent { get; }
        IBehaviorModel Model { get; }
        bool IsStatic { get; }
    }

    internal interface IInternalBehavior : IBehavior
    {
        CardInstance RealHost { get; set; }
        void Initialize(IBehaviorModel model, bool persistent);
        void TransferFrom(IBehavior original);
        void ReceiveMessage(int messageId, object arg);
    }
}
