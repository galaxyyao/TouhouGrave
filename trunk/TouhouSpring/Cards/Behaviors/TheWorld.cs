using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
	public class TheWorld : BaseBehavior<TheWorld.ModelType>, ICastableSpell
	{
        public bool Cast(Game game, out string reason)
		{
			if (Host.Owner.Mana < Model.ManaCost)
			{
				reason = "Insufficient mana";
				return false;
			}

			var silence = new Silence();
			var lastingEffect = new LastingEffect(Model.Lasting);
			lastingEffect.CleanUps.Add(silence);
			Host.Behaviors.Add(silence);
			Host.Behaviors.Add(lastingEffect);
			game.UpdateMana(Host.Owner, -Model.ManaCost);
			reason = string.Empty;
			return true;
		}

        [BehaviorModel("The World", typeof(TheWorld))]
        public class ModelType : BehaviorModel
        {
            public int ManaCost { get; set; }
            public int Lasting { get; set; }
        }
	}
}
