using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TouhouSpring.Commands
{
    public class CastSpell : BaseCommand
    {
        // TODO: change to serialization-friendly ID
        public Behaviors.IBehavior Spell
        {
            get; private set;
        }

        public CastSpell(Behaviors.IBehavior spell)
        {
            if (spell == null)
            {
                throw new ArgumentNullException("spell");
            }

            Spell = spell;
        }

        internal override void ValidateOnIssue()
        {
            Validate(Spell);
            if (!(Spell is Behaviors.ICastableSpell))
            {
                FailValidation("Spell is not a spell."); //...
            }
        }

        internal override void ValidateOnRun()
        {
        }

        internal override void RunMain()
        {
            Debug.Assert(Game.RunningCommand == this);
            (Spell as Behaviors.ICastableSpell).Run(this);
        }
    }
}
