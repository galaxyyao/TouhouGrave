using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
	public partial class BaseCard
	{
		/// <summary>
		/// A collection of behaviors boudn to this card
		/// </summary>
		public Behaviors.BehaviorList Behaviors
		{
			get; private set;
		}

		/// <summary>
		/// Get a collection of behaviors which implements ISpell.
		/// </summary>
		/// <returns>The collection</returns>
		public IEnumerable<Behaviors.ICastableSpell> GetSpells()
		{
			return Behaviors.OfType<Behaviors.ICastableSpell>();
		}

		/// <summary>
		/// Trigger ITrigger behaviors given the context object.
		/// </summary>
		/// <typeparam name="TContext">The trigger context type</typeparam>
		/// <param name="context">The context object</param>
		/// <returns>True if all specific-typed behavior objects have went through, i.e. no early return; false otherwise</returns>
		internal bool Trigger<TContext>(TContext context)
			where TContext : Triggers.TriggerContext
		{
			return Trigger(context, t => false);
		}

		/// <param name="earlyQuitPredicate">The predicate determining whether an early quit shall be performed</param>
		internal bool Trigger<TContext>(TContext context, Predicate<TContext> earlyQuitPredicate)
			where TContext : Triggers.TriggerContext
		{
			Debug.Assert(context != null);

			var copies = Behaviors.ToArray();
			foreach (var bhv in copies)
			{
				if (bhv is Behaviors.ITrigger<TContext>)
				{
					(bhv as Behaviors.ITrigger<TContext>).Trigger(context);
				}

				if (earlyQuitPredicate(context))
				{
					return false;
				}
			}

			return true;
		}
	}
}
