using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
	/// <summary>
	/// The interface of a trigger type
	/// </summary>
	/// <typeparam name="TContext">The trigger context type</typeparam>
	public interface ITrigger<TContext> : IBehavior
		where TContext : Triggers.TriggerContext
	{
		void Trigger(TContext context);
	}
}
