using System;
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

        void OnMessage(string message, object[] args);
	}

    internal interface IInternalBehavior : IBehavior
    {
        void Initialize(BehaviorModel model, bool persistent);
        void Bind(BaseCard host);
        void Unbind();
    }
}
