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
	}
}
