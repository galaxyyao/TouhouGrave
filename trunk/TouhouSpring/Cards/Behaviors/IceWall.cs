using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TouhouSpring.Triggers;

namespace TouhouSpring.Behaviors
{
	public class IceWall : BaseBehavior<IceWall.ModelType>, ITrigger<PreCardDamageContext>
	{
        public void Trigger(PreCardDamageContext context)
		{
			if (context.CardToDamage == Host)
			{
				if (Host.Owner.Mana < Model.ManaCost)
				{
					context.Cancel = true;
					context.Reason = "Insufficient mana";
				}
				else
				{
					var result = new Interactions.MessageBox(context.Game.OpponentController
						, "施放冰墙么？"
						, Interactions.MessageBox.Button.Yes | Interactions.MessageBox.Button.No).Run();
					if (result == Interactions.MessageBox.Button.Yes)
					{
						context.Game.UpdateMana(Host.Owner, -Model.ManaCost);
						context.DamageToDeal = (context.DamageToDeal == 1) ? 0 : context.DamageToDeal - 2;
					}
				}
			}
		}

        [BehaviorModel(typeof(IceWall), DefaultName = "冰墙")]
        public class ModelType : BehaviorModel
        {
            public int ManaCost { get; set; }
        }
	}
}
