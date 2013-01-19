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

        internal NotifySpellEvent(Game game, string notification, Behaviors.ICastableSpell spell)
            : this(game, notification, spell, null)
        { }

        internal NotifySpellEvent(Game game, string notification, Behaviors.ICastableSpell spell, string message)
            : base(game, notification)
        {
            Debug.Assert(spell != null);
            Spell = spell;
            Message = message;
        }
    }
}
