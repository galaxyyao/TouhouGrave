using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TouhouSpring.Commands
{
    public class CastSpell : ICommand
    {
        public string Token
        {
            get { return "CastSpell"; }
        }

        public Behaviors.ICastableSpell Spell
        {
            get; set;
        }

        public void Validate(Game game)
        {
            if (Spell == null)
            {
                throw new CommandValidationFailException("Spell can't be null.");
            }
        }

        public void RunMain(Game game)
        {
            Debug.Assert(game.RunningCommand.Command == this);
            Spell.Run(game.RunningCommand as CommandContext<CastSpell>);
        }
    }
}
