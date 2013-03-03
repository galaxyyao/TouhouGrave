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
    }

    internal interface IInternalBehavior : IBehavior
    {
        void Initialize(IBehaviorModel model, bool persistent);
        void TransferFrom(IBehavior original);
        void Bind(CardInstance host);
        void Unbind();
        void ReceiveMessage(string message, object[] args);
    }
}
