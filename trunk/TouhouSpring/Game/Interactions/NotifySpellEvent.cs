using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TouhouSpring.Interactions
{
	public class NotifySpellEvent : NotifyOnly
	{
		public Behaviors.ICastableSpell Spell
		{
			get; private set;
		}

		public string Message
		{
			get; private set;
		}

		internal NotifySpellEvent(BaseController controller, string notification, Behaviors.ICastableSpell spell)
			: this(controller, notification, spell, null)
		{ }

		internal NotifySpellEvent(BaseController controller, string notification, Behaviors.ICastableSpell spell, string message)
			: base(controller, notification)
		{
			Debug.Assert(spell != null);
			Spell = spell;
			Message = message;
		}
	}
}
