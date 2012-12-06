using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using TouhouSpring.Interactions;

namespace TouhouSpring
{
	public partial class Game
	{
		public void CastSpell(Behaviors.ICastableSpell spell)
		{
			if (spell == null)
			{
				throw new ArgumentNullException("spell");
			}

			string reason = null;
			if (spell.Cast(this, out reason))
			{
				m_controllers.ForEach(c => c.InternalOnSpellCasted(spell));
			}
			else
			{
				m_controllers.ForEach(c => c.InternalOnSpellCastCanceled(spell, reason));
			}
		}
	}
}
