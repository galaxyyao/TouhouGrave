﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public interface IBehavior
    {
        BaseCard Host { get; }
        bool Persistent { get; }
        BehaviorModel Model { get; }
    }

    internal interface IInternalBehavior : IBehavior
    {
        void Initialize(BehaviorModel model, bool persistent);
        void TransferFrom(IBehavior original);
        void Bind(BaseCard host);
        void Unbind();
        void ReceiveMessage(string message, object[] args);
    }
}
